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