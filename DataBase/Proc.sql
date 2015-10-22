create PROCEDURE [dbo].ObjHost$FindOrCreate
	@sName nvarchar(128),
	@sCluster nvarchar(128)
AS
begin

	DECLARE @hostId bigint = (select top(1) id from ObjHost where sName = @sName and sCluster=@sCluster)
	
	IF @hostId is NULL 	
	BEGIN
		insert into ObjHost(sName,sCluster) values(@sName,@sCluster)

		set @hostId = SCOPE_IDENTITY()
		select id,dtCreate from ObjHost where id = @hostId
	END
	ELSE BEGIN
	   select id,dtCreate from ObjHost where id = @hostId
	END
END

go

create PROCEDURE [dbo].ObjHost$Del
	@id bigint
AS
begin

	delete from ObjHost where id=@id

END

go
create PROCEDURE [dbo].ObjSource$FindOrCreate
	@idHost bigint,
	@sUrl nvarchar(max),
	@fIsPush bit
AS
begin

	DECLARE @id bigint = (select top(1) id from ObjSource where idHost = @idHost and sUrl=@sUrl and fIsPush=@fIsPush)
	
	IF @id is NULL 	
	BEGIN
		insert into ObjSource(idHost,sUrl,fIsPush) values(@idHost,@sUrl,@fIsPush)

		set @id = SCOPE_IDENTITY()
	END

	exec ObjSource$Get @id
END

go

create PROCEDURE [dbo].ObjSource$Get
	@id bigint
AS
begin
	SELECT 
		id,
		[idHost]
      ,[sUrl]
      ,[fIsPush]
      ,[dtCreate]
      ,[dtRunLast]
      ,[dtRunNext]
      ,[dtCheckout]
      ,[nDelaySec]
      ,[nRetry]
      ,[sError]
	FROM [ObjSource]
	where id=@id

END
go

create PROCEDURE [dbo].ObjSource$Checkout	
AS
begin

	DECLARE @id bigint = (select top(1) id from ObjSource where fIsPush=0 and dtCheckout is null and (dtRunNext is null or dtRunNext>GetUTCDate()) order by nRetry)
	
	IF @id is not NULL 	
	BEGIN
		
		update ObjSource
		set dtCheckout = GetUTCDate()
		where id = @id

		exec ObjSource$Get @id	
	END
	
END

go

create PROCEDURE [dbo].ObjSource$Checkin
	@id bigint,
	@sContent nvarchar(max)
AS
begin

	update ObjSource
	set 
		dtCheckout = null,
		dtRunLast = getutcdate(),
		dtRunNext = DATEADD(s, nRetry, getutcdate()),
		nRetry = 0
	where id = @id
	
	exec ObjResult$Apply @id, @sContent
	exec Result$Archive$Insert @id, @sContent
END

go

create PROCEDURE ObjResult$Apply
	@idSource bigint,
	@sContent nvarchar(max)
AS
begin

	DECLARE @id bigint = (select top(1) @idSource from ObjResult where idSource = @idSource)
	
	IF @id is NULL BEGIN
		insert into ObjResult(idSource,sContent,dtUpdate) values(@idSource,@sContent,getUtcDate())
	END
	else begin
		update ObjResult
		set sContent = @sContent,
			dtUpdate = getUtcDate()
		where idSource = @idSource
	end
END

go

create PROCEDURE ObjResult$All
AS
begin

	SELECT R.[idSource]
      ,R.[sContent]
      ,R.[dtCreate]
      ,R.[dtUpdate]
	  ,S.idHost
  FROM [ObjResult] R inner join ObjSource S on R.idSource = S.id

END

go

create PROCEDURE [dbo].Result$Archive$Insert
	@idSource bigint,
	@sContent nvarchar(max)
AS
begin

	insert into Result$Archive(idSource,sContent) values(@idSource,@sContent)

	-- чистим архив
	exec Result$Archive$Clear 7
END

go

create PROCEDURE [dbo].[ObjSource$CheckinError]
	@id bigint,
	@sError nvarchar(max)
AS
begin

	update ObjSource
	set 
		dtCheckout = null,
		nRetry = nRetry+1,
		sError = @sError
	where id = @id

END
go
create PROCEDURE [dbo].[Result$Archive$Clear]
	@nDays int
AS
begin
	declare @curDate datetime = getutcdate()
	
	delete from Result$Archive
	where datediff(d,dtCreate, @curDate) > @nDays
end

go

create PROCEDURE [dbo].[ObjSource$Checkout_Reset]	
AS
begin
		
	update ObjSource
	set dtCheckout = null
	where dtCheckout is not null
	
END

go

create PROCEDURE [dbo].[ObjSource$Del]	
	@id bigint
AS
begin
		
	delete from ObjSource
	where id = @id
	
END