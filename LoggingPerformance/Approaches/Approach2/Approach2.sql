create table ActivityLog_Approach2
(
	Id nvarchar(50) not null primary key clustered,
	LogData varbinary(max) not null,
	[Format] nvarchar(10) null,
	JSON ntext
)
go

