create table ActivityLog_Approach3
(
	Id nvarchar(50) not null primary key clustered,
	LogData nvarchar(max) not null,
	[Format] nvarchar(10) null,
	JSON nvarchar(max)
)
go

