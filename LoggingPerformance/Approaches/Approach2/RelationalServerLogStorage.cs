using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LoggingPerformance.Octopus;
using LoggingPerformance.Octopus.Persistance;
using LoggingPerformance.TestHarness;

namespace LoggingPerformance.Approaches.Approach2
{
    public class RelationalServerLogStorage : IServerLogStorage, IDisposable
    {
        readonly IRelationalStore store;
        readonly ConcurrentDictionary<string, Lazy<SqlLogAgent>> readerWriters = new ConcurrentDictionary<string, Lazy<SqlLogAgent>>();
        readonly ReaderWriterLockSlim cleanupLock = new ReaderWriterLockSlim();

        public RelationalServerLogStorage(IRelationalStore store)
        {
            this.store = store;
        }

        public void Append(string correlationId, ActivityLogEntry entry)
        {
            cleanupLock.EnterReadLock();
            try
            {
                var id = GetLogId(correlationId);
                var writer = readerWriters.GetOrAdd(id, i => new Lazy<SqlLogAgent>(() => new SqlLogAgent(i, store, () => Expired(i))));
                writer.Value.Append(correlationId, entry);
            }
            finally
            {
                cleanupLock.ExitReadLock();
            }
        }

        public IList<ActivityLogTreeNode> GetLog(string correlationId)
        {
            cleanupLock.EnterReadLock();
            try
            {
                var id = GetLogId(correlationId);
                var writer = readerWriters.GetOrAdd(id, i => new Lazy<SqlLogAgent>(() => new SqlLogAgent(i, store, () => Expired(i))));
                return new[] { writer.Value.Build() };
            }
            finally
            {
                cleanupLock.ExitReadLock();
            }
        }

        void Expired(string id)
        {
            cleanupLock.EnterWriteLock();
            try
            {
                Lazy<SqlLogAgent> agent;
                if (readerWriters.TryRemove(id, out agent))
                {
                    if (agent.IsValueCreated)
                    {
                        agent.Value.Dispose();
                    }
                }
            }
            finally
            {
                cleanupLock.ExitWriteLock();
            }
        }

        static string GetLogId(string correlationId)
        {
            return correlationId.Split('/').FirstOrDefault() ?? correlationId;
        }

        public void ForceFlush(string correlationId)
        {
            cleanupLock.EnterReadLock();
            try
            {
                var id = GetLogId(correlationId);
                var writer = readerWriters.GetOrAdd(id, i => new Lazy<SqlLogAgent>(() => new SqlLogAgent(i, store, () => Expired(i))));
                writer.Value.ForceFlush();
            }
            finally
            {
                cleanupLock.ExitReadLock();
            }
        }

        public class SqlLogAgent
        {
            readonly object sync = new object();
            readonly Action expiredCallback;
            readonly string id;
            readonly IRelationalStore store;
            readonly Timer timer;
            readonly ActivityLogApproach2 activityLogApproach2;
            readonly ActivityLogTreeNode treeNode;
            DateTimeOffset lastWritten;
            bool hasBeenWritten;
            bool dirty;
            bool disposed;
            bool isNew;
            bool warningOrError;
            bool warningOrErrorWritten;

            public SqlLogAgent(string id, IRelationalStore store, Action expiredCallback)
            {
                this.id = id;
                this.store = store;
                this.expiredCallback = expiredCallback;

                lastWritten = DateTimeOffset.UtcNow;
                timer = new Timer(FlushTimerCallback);
                timer.Change(1000, Timeout.Infinite);

                using (var transaction = store.BeginTransaction())
                {
                    activityLogApproach2 = transaction.Load<ActivityLogApproach2>(id);
                    if (activityLogApproach2 == null)
                    {
                        isNew = true;
                        activityLogApproach2 = new ActivityLogApproach2(id);
                    }
                }

                treeNode = activityLogApproach2.Deserialize();
            }

            public ActivityLogTreeNode Build()
            {
                lock (sync)
                {
                    return treeNode.Clone();
                }
            }

            public void Append(string correlationId, ActivityLogEntry entry)
            {
                Timings.Increment("Log appends");
                lock (sync)
                {
                    var node = treeNode.FindOrAdd(correlationId);
                    node.LogEntries.Add(entry);
                    dirty = true;
                    hasBeenWritten = true;
                    lastWritten = DateTimeOffset.UtcNow;
                    if (entry.Category == ActivityLogEntryCategory.Warning || entry.Category == ActivityLogEntryCategory.Fatal || entry.Category == ActivityLogEntryCategory.Error)
                    {
                        warningOrError = true;
                    }
                }
            }

            bool HasExpired()
            {
                return DateTimeOffset.UtcNow > lastWritten.AddSeconds(hasBeenWritten ? 10 : 2);
            }

            void FlushTimerCallback(object state)
            {
                if (disposed)
                    return;

                try
                {
                    if (HasExpired())
                    {
                        expiredCallback();
                        return;
                    }

                    if (!dirty)
                    {
                        timer.Change(1000, Timeout.Infinite);
                        return;
                    }

                    Flush();

                    timer.Change(1000, Timeout.Infinite);
                }
                catch (ObjectDisposedException)
                {
                }
            }

            public void Flush()
            {
                lock (sync)
                {
                    if (!dirty)
                        return;

                    Timings.Increment("Flush to disk");

                    try
                    {
                        using (var transaction = store.BeginTransaction())
                        {
                            activityLogApproach2.Serialize(treeNode);

                            if (isNew)
                            {
                                transaction.Insert(activityLogApproach2, id);
                                isNew = false;
                            }
                            else
                            {
                                transaction.Update(activityLogApproach2);
                            }

                            // TODO: Disabled for test
                            //if (warningOrError && !warningOrErrorWritten)
                            //{
                            //    transaction.ExecuteScalar<int>("UPDATE [ServerTask] set HasWarningsOrErrors = 1 WHERE Id = @id", new CommandParameters { { "id", id } });
                            //    warningOrErrorWritten = true;
                            //}

                            transaction.Commit();
                        }

                        dirty = false;
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("Unable to save activity log: " + ex.Message);
                    }
                }
            }

            public void Dispose()
            {
                disposed = true;
                timer.Dispose();
                Flush();
            }

            public void ForceFlush()
            {
                dirty = true;
                Flush();
            }
        }

        public void Dispose()
        {
            cleanupLock.EnterWriteLock();
            try
            {
                foreach (var item in readerWriters.Values)
                {
                    if (item.IsValueCreated)
                    {
                        item.Value.Dispose();
                    }
                }

                readerWriters.Clear();
            }
            finally
            {
                cleanupLock.ExitWriteLock();
            }
        }
    }
}
