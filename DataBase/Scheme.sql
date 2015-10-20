create table ObjHost
(
	id bigint identity(1,1) primary key,
	sName nvarchar(128) not null,
	sCluster nvarchar(128) not null,
	dtCreate datetime not null default(GetUtcDate())
)
go
create nonclustered index IDX_ObjHost_NameCluster on ObjHost(sName,sCluster)
go
--drop table ObjSource
go
create table ObjSource
(
	id bigint identity(1,1) primary key,
	idHost bigint not null,
	sUrl nvarchar(max) not null,
	fIsPush bit not null,
	dtCreate datetime not null default(GetUtcDate()),
	dtRunLast datetime,
	dtRunNext datetime,
	dtCheckout datetime,
	nDelaySec int not null default(3600),
	nRetry int not null default(0),
	sError nvarchar(max)
)

go

create nonclustered index IDX_Source_Host on ObjSource(idHost)

go
--drop table ObjResult
go
create table ObjResult
(
	idSource bigint primary key,
	sContent nvarchar(max) not null,
	dtCreate datetime not null default(GetUtcDate()),
	dtUpdate datetime not null
)
go
create table Result$Archive
(
	id bigint identity(1,1) primary key,
	idSource bigint not null,
	sContent nvarchar(max) not null,
	dtCreate datetime not null default(GetUtcDate())
)
go
create nonclustered index IDX_Result$Archive_Create on Result$Archive(dtCreate)