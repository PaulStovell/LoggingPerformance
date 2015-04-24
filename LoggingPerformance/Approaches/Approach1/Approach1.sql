create table ActivityLog_Approach1
(
	Id nvarchar(50) not null constraint PK_ActivityLog_Id primary key clustered,
	LogData varbinary(max) not null,
	[Format] nvarchar(10) null,
	JSON ntext
)
go

