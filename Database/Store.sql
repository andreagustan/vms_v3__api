USE [VMS_V3]
GO
/****** Object:  UserDefinedFunction [dbo].[FnJsonFilter]    Script Date: 17/11/2023 11:03:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [dbo].[FnJsonFilter](	
	@JsonString nvarchar(max)
)
returns nvarchar(max)
as
begin
	declare @wherecondition nvarchar(max)=''

	set @wherecondition=STUFF((
		select ' and '+
			case when [key] like '%|like%' then concat(replace([key],'|like', ' like '), '''%'+[value]+'%'' ')
				when [key] like '%|morethan%' then concat(replace([key],'|morethan', ' >= '), [value] )
				when [key] like '%|lessthan%' then concat(replace([key],'|lessthan', ' <= '), [value])
				when [key] like '%|in%' then concat(replace([key],'|in', ' in '), '('+ 
				replace(replace(replace([value],'[',''),']',''),'"','''') + ')')
				when [key] like '%|notin%' then concat(replace([key],'|notin', ' not in '),'('+ 
				replace(replace(replace([value],'[',''),']',''),'"','''') + ')')
			else concat([key], ' = ', ''''+[value]+''' ') end
		FROM OPENJSON(@JsonString) js
	for xml path('')), 1, 4, '')

	set @wherecondition = replace(replace(@wherecondition,'&gt;', '>'),'&lt;', '<')
	
	return @wherecondition
end
GO
/****** Object:  StoredProcedure [dbo].[GetDataList]    Script Date: 17/11/2023 11:03:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[GetDataList] @userlogin varchar(100), @code varchar(100)
, @combojson nvarchar(max)=null
, @filterjson nvarchar(max)=null
, @page int = 1, @size int = 20
, @order nvarchar(max) = '', @search nvarchar(max)=''  
as

--declare @userlogin varchar(100) = 'dev.andre', @code varchar(100) ='M_pricelistvoucher', @status varchar(100)=''
--, @combojson nvarchar(max)=null
--, @findjson nvarchar(max)=null
--, @filterjson nvarchar(max)='{"supplierID":"PP0022","itemID":"21201001","beginQty__lessthan":"4","endQty__morethan":"4"}'
--, @page int = 1, @size int = 20
--, @order nvarchar(max) = '', @search nvarchar(max)=''  

--DECLARE
--@AllowAllAction INT,
--@AllowApprove INT,
--@AllowReject INT,
--@AllowDelete INT,
--@AllowResubmit INT

--SET @AllowAllAction = ( SELECT SSO.dbo.fnGetExtendAcessModul(@userlogin, 163))
--SET @AllowApprove = ( SELECT SSO.dbo.fnGetExtendAcessModul(@userlogin, 164))

declare @combo nvarchar(max)='', @comboselect nvarchar(max)=''
	if isnull(@combojson,'') != '' begin
	set @combo = 
		isnull(STUFF((
			SELECT distinct
			  char(10)+' '+concat('LEFT JOIN (select * from ', JSON_VALUE(js.[value], '$.combo'), ') as ', js.[key], '_combo ', 'on a.', js.[key], ' = ', js.[key], '_combo.', JSON_VALUE(js.[value], '$.keys')) 
			FROM OPENJSON(@combojson) js
			CROSS APPLY OPENJSON(js.[value]) j	
		for xml path('')), 1, 2, ''), '') 

	set @comboselect =
	isnull(STUFF((
		SELECT distinct ', '
		--+concat(js.[key], '_combo.', JSON_VALUE(js.[value], '$.keys'), ' [', js.[key] ,'_key]') + ', ' +
		+concat(js.[key], '_combo.', JSON_VALUE(js.[value], '$.values'), ' [', js.[key] ,'Name]')
		FROM OPENJSON(@combojson) js
		CROSS APPLY OPENJSON(js.[value]) j	
	for xml path('')), 1, 2, ''), '') 
end

declare @wherecondition nvarchar(max)=''
if isnull(@filterjson,'') != '' begin
	set @wherecondition = 
	isnull(STUFF((
		select ' and '+
			case when [key] like '%__like%' then concat(replace([key],'__like', ' like '), '''%'+[value]+'%'' ')
			 when [key] like '%__morethan%' then concat(replace([key],'__morethan', ' >= '), [value] )
			 when [key] like '%__lessthan%' then concat(replace([key],'__lessthan', ' <= '), [value])
			else concat([key], ' = ', ''''+[value]+''' ') end
		FROM OPENJSON(@filterjson) js
	for xml path('')), 1, 4, ''), '') 
	
	set @wherecondition = replace(replace(replace(@wherecondition, '__', ' '),'&lt;', '<'), '&gt;', '>')
end

declare @strcol nvarchar(max)='', @col nvarchar(max)=''
set @strcol = 
'
set @col= 
 STUFF((
	 SELECT '', '' + 
			case when data_type = ''date'' then concat(''format('', concat(''a.'', quotename(COLUMN_NAME)), '',''''yyyy/MM/dd'''') '', quotename(COLUMN_NAME))
			else concat(''a.'', quotename(COLUMN_NAME)) end
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = '''+@code+'''
FOR XML PATH('''')), 1, 2, '''') 

'
EXEC sp_executesql @strcol, N'@col nvarchar(max) OUTPUT', @col = @col OUTPUT;
print  concat('col : ' ,@col)

declare @colsearch nvarchar(max) = ''
if @search <> '' begin 
set @strcol = ''
set @strcol = 
'
set @col= 
 STUFF((
	 SELECT ''or '' + 
			case when data_type = ''date'' then concat(''format('', concat(''a.'', quotename(COLUMN_NAME)), '',''''yyyy/MM/dd'''') '', quotename(COLUMN_NAME) )
			else concat(''convert(nvarchar(max), '', ''a.'', quotename(COLUMN_NAME), '')'', '' LIKE '' , ''''''%'', '''+@search+''', ''%'''' '') end
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = '''+@code+'''
FOR XML PATH('''')), 1, 2, '''') 

'
EXEC sp_executesql @strcol, N'@col nvarchar(max) OUTPUT', @col = @colsearch OUTPUT;
--print @strcol
set @colsearch = concat(' and (', @colsearch, ')')
print  concat('colsearch : ' ,@colsearch)
end



declare @sqlstr nvarchar(max) = ''

/*cek createdate, createuser ada atau tidak*/
declare @sqlstrRn nvarchar(max)='', @sqlstrCU nvarchar(max)
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = @code  AND COLUMN_NAME = 'CreateDate') begin 
    set @sqlstrRn = 'ROW_NUMBER() OVER(ORDER BY a.createdate ASC) rn,'
	+'''Create By. ''+ A.CreateUser + '' On: ''+ convert(varchar, A.CreateDate, 9) AS Creator, '
end
else begin
	declare @fcol nvarchar(100)=''
	SELECT top 1 @fcol = COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Temp_UserId' ORDER BY ORDINAL_POSITION
	set @sqlstrRn = 
	'ROW_NUMBER() OVER(ORDER BY a.'+@fcol+' ASC) rn, '
end
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = @code  AND COLUMN_NAME = 'createuser') begin 
    set @sqlstrCU = iif(isnull(@userlogin,'') = 'alluser', '' , 'and a.createuser = '''+@userlogin+'''')
end
else begin
	set @sqlstrCU = ''
end
set @sqlstr = 
'
select concat(''recid-'', rn) recid, * from (
	SELECT 
	'+@sqlstrRn+'
	'+@col+

	+iif(isnull(@comboselect,'')<>'', ', '+@comboselect, '')+'
	FROM '+@CODE+' a
	 '+isnull(@COMBO,'')+'
	where 1 = 1
	'+@sqlstrCU+'
'+iif(isnull(@wherecondition,'')<>'','and '+@wherecondition,'')+ 
@colsearch+
') as sub
where rn between '+convert(varchar(5),@page)+' and '+convert(varchar(5),@page*@size)+''


print concat('combo : ' ,@combo)	
PRINT concat('query :' ,@SQLSTR)	

exec (@sqlstr)

GO
/****** Object:  StoredProcedure [dbo].[M_CustomerDetail_save]    Script Date: 17/11/2023 11:03:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[M_CustomerDetail_save] @userlogin nvarchar(50), @mode nvarchar(100), @formdata nvarchar(max)
as
	--declare @userlogin nvarchar(50)='dev.andre', @mode nvarchar(100)='edit', @formdata nvarchar(max) = '	{"Id":"2","customerID":"CRM2021080097","customerName":"","subCustomerID":"214214","subCustomerName":"aqwrqwrwqcvqvqvv "}'


	declare @msg nvarchar(max)= '', @id int=0
if @mode = 'add' begin
	insert into M_CustomerDetail (customerid, customername, subcustomerid, subcustomername, createuser, createdate, isdeleted)
	select  customerid, customerName, subcustomerid, subcustomername, @userlogin, getdate(), 0
	FROM OPENJSON(@formdata, '$') 
	WITH (
		customerID nvarchar(100), customerName nvarchar(300), subCustomerID nvarchar(100),  subCustomerName nvarchar(300)
	) AS a
	SET @id = SCOPE_IDENTITY();
	select @msg = 'Success'
end
else if @mode = 'edit' begin

	select @id = id
	FROM OPENJSON(@formdata, '$') 
	WITH (Id int) AS a

	update b
	set 
	b.customerid = a.customerid, b.customerName = a.customerName, b.subcustomerid = a.subcustomerid, b.subcustomername = a.subcustomername
	, b.updatedate=getdate(), b.updateuser = @userlogin
	FROM OPENJSON(@formdata, '$') 
	WITH (
		Id int,
		customerID nvarchar(100), customerName nvarchar(300), subCustomerID nvarchar(100),  subCustomerName nvarchar(300)
	) AS a
	inner join M_CustomerDetail b on a.id = b.id

	select @msg = 'Success'
end
	
	select @msg msg, convert(varchar(5),@id) id
GO
/****** Object:  StoredProcedure [dbo].[M_PriceListVoucher_save]    Script Date: 17/11/2023 11:03:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[M_PriceListVoucher_save] @userlogin nvarchar(50), @mode nvarchar(100), @formdata nvarchar(max)
as
	--{"supplierID":"PP0021","itemID":"00100537","beginQty":"10","endQty":"100","price":"50000"}
	declare @msg nvarchar(max)= '', @id int=0
if @mode = 'add' begin
	insert into M_PriceListVoucher (supplierid, itemid, beginqty, endqty, price, createuser, createdate, isdeleted)
	select  supplierid, itemid, beginqty, endqty, price, @userlogin, getdate(), 0
	FROM OPENJSON(@formdata, '$') 
	WITH (
		supplierID nvarchar(100), itemID nvarchar(100), beginQty int, endQty int, price float
	) AS a
	SET @id = SCOPE_IDENTITY();
	select @msg = 'Success'
end
else if @mode = 'edit' begin

	select @id = id
	FROM OPENJSON(@formdata, '$') 
	WITH (Id int) AS a

	update b
	set b.supplierid = a.supplierid, b.itemid = a.itemid, b.beginqty = a.beginqty, b.endqty = a.endqty, b.price=a.price
	, b.updatedate=getdate(), b.updateuser = @userlogin
	FROM OPENJSON(@formdata, '$') 
	WITH (
		Id int, supplierID nvarchar(100), itemID nvarchar(100), beginQty int, endQty int, price float
	) AS a
	inner join m_pricelistvoucher b on a.id = b.id

	select @msg = 'Success'
end
	
	select @msg msg, @id id
GO
/****** Object:  StoredProcedure [dbo].[ProcCRUDCustomerDetail]    Script Date: 17/11/2023 11:03:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Umar>
-- Create date: <9 Nov 2023>
-- Description:	<Proc CRUD Customer Detail>
-- =============================================
CREATE PROCEDURE [dbo].[ProcCRUDCustomerDetail] 
	@UserId varchar(100)=null,
	@JsonData nvarchar(max)=null,
	@JsonFilter nvarchar(max)=null,
	@Field nvarchar(100)=null,
	@KeyWord nvarchar(max)=null,	
	@PageNumber nvarchar(200)=null ,
    @PageSize nvarchar(200)=null ,
	@totalrecords INT OUTPUT,
	@totalrecordFilter INT OUTPUT
AS
BEGIN

	SET NOCOUNT ON;
	begin try
		declare @command nVARCHAR(MAX), @commandfilter nVARCHAR(MAX)= '', @Msg nvarchar(10)='',@commandcount nVARCHAR(MAX),
		@BykDataAdd bigint=0, @BykDataEdit bigint=0, @BykDataDelete bigint=0,@wherecondition nvarchar(max)=''
		
		declare @TblData table(Id bigint, customerID nvarchar(200), customerName nvarchar(100), 
		subCustomerID nvarchar(100), subCustomerName nvarchar(200), FlagData nvarchar(10))

		if @JsonData != 'null'
		begin
			insert into @TblData
			select * from OPENJSON(@JsonData) WITH(Id bigint, customerID nvarchar(200), customerName nvarchar(100), 
			subCustomerID nvarchar(100), subCustomerName nvarchar(200), FlagData nvarchar(10))

			set @BykDataAdd=(select count(Id) from @TblData where FlagData='Add')
			set @BykDataEdit=(select count(Id) from @TblData where FlagData='Edit')
			set @BykDataDelete=(select count(Id) from @TblData where FlagData='Delete')

			if @BykDataAdd != 0
			begin
				insert into M_CustomerDetail(customerID, customerName, subCustomerID, subCustomerName, CreateDate, CreateUser)
				select customerID, customerName, subCustomerID, subCustomerName, getdate(), @UserId from @TblData where FlagData='Add'
			end
			if @BykDataEdit != 0
			begin
				update a set a.UpdateDate=getdate(), a.UpdateUser=@UserId, a.customerID=b.customerID, 
				a.customerName=b.customerName, a.subCustomerID=b.subCustomerID, a.subCustomerName=b.subCustomerName
				from M_CustomerDetail a inner join (select * from @TblData where FlagData='Edit') b
				on a.Id=b.Id
			end
			if @BykDataDelete != 0
			begin
				update a set a.DeletedDate=getdate(), a.DeletedUser=@UserId, a.IsDeleted=1
				from M_CustomerDetail a inner join (select * from @TblData where FlagData='Delete') b
				on a.Id=b.Id
			end

			set @Msg=(
			select case when @BykDataDelete != 0 and @BykDataEdit != 0 and @BykDataAdd != 0 then 'Bulk'
				when @BykDataDelete != 0 and @BykDataAdd != 0 then 'Bulk'
				when @BykDataDelete != 0 and @BykDataEdit != 0 then 'Bulk'
				when @BykDataAdd != 0 and @BykDataEdit != 0 then 'Bulk'
				when @BykDataDelete != 0 then 'Delete'
				when @BykDataEdit != 0 then 'Update'
				when @BykDataAdd != 0 then 'Create' end
			)

			select 'Success '+ @Msg +' Action.' as Result;
		end
		else if @Field='GetById'
		begin
			select *
			,'Create by ' + CreateUser +  ' On : ' + CONVERT(varchar,CreateDate,121) as Creator
			,'Update by ' + UpdateUser +  ' On : ' + CONVERT(varchar,UpdateDate,121) as Updater
			from M_CustomerDetail where IsDeleted=0 and Id=@KeyWord
		end
		else
		begin
			set @command=N'select *
			,''Create by '' + CreateUser +  '' On : '' + CONVERT(varchar,CreateDate,121) as Creator
			,''Update by '' + UpdateUser +  '' On : '' + CONVERT(varchar,UpdateDate,121) as Updater
			from M_CustomerDetail where IsDeleted=0 '

			set @commandcount = N'select @Tot=count(IsDeleted) from M_CustomerDetail where IsDeleted=0 '

			set @totalrecords = (select count(IsDeleted) from M_CustomerDetail where IsDeleted=0)
						
			if @KeyWord is not null
			begin
				set @commandfilter=N'and (subCustomerName like ''%'+ @KeyWord +'%'' or subCustomerID like ''%'+ @KeyWord +'%''
				or customerName like ''%'+ @KeyWord +'%'' or customerID like ''%'+ @KeyWord +'%'') '
			end

			if @JsonFilter is not null
			begin
				set @wherecondition=(select dbo.FnJsonFilter(@JsonFilter))

				set @commandfilter = 'and ' + @wherecondition

			end

			if @PageNumber is not null
			begin
				if @PageSize='All' or @PageSize is null
				begin
				 set @PageSize = @totalrecords
				end

				set @command = @command + ISNULL(@commandfilter, '')
				+ ' ORDER BY Id OFFSET ' + CONVERT(VARCHAR, @PageNumber)
				+ ' ROWS FETCH NEXT ' + CONVERT(VARCHAR, @PageSize)
				+ ' ROWS ONLY  '

				set @commandcount=@commandcount + ISNULL(@commandfilter,'')
			end
			else
			begin
				set @command = @command + ISNULL(@commandfilter, '')
				set @commandcount=@commandcount + ISNULL(@commandfilter,'')
			end
						
			exec Sp_executesql @commandcount, N'@Tot nvarchar(100) OUTPUT',  @Tot=@totalrecordFilter OUTPUT;
			exec (@command)
			
			select @totalrecords as TotalRecord, @totalrecordFilter as TotalFilter
		end		
		
	end try
	BEGIN CATCH

        SELECT 'Error : ' + ERROR_MESSAGE() as Result;

    END CATCH;
END
GO
/****** Object:  StoredProcedure [dbo].[ProcCRUDPriceListVoucher]    Script Date: 17/11/2023 11:03:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Umar>
-- Create date: <9 Nov 2023>
-- Description:	<Proc CRUD Price List Voucher>
-- =============================================
CREATE PROCEDURE [dbo].[ProcCRUDPriceListVoucher] 
	@UserId varchar(100)=null,
	@JsonData nvarchar(max)=null,
	@JsonFilter nvarchar(max)=null,
	@Field nvarchar(100)=null,
	@Qty bigint=null,
	@ItemId nvarchar(100)=null,
	@SupplierID nvarchar(100)=null,
	@KeyWord nvarchar(max)=null,
	@KeyHeader nvarchar(max)=null,
	@PageNumber nvarchar(200)=null ,
    @PageSize nvarchar(200)=null ,
	@totalrecords INT OUTPUT,
	@totalrecordFilter INT OUTPUT
AS
BEGIN

	SET NOCOUNT ON;
	begin try
		declare @command nVARCHAR(MAX), @commandfilter nVARCHAR(MAX)= '', @Msg nvarchar(10)='',@wherecondition nvarchar(max)='',
		@BykDataAdd bigint=0, @BykDataEdit bigint=0, @BykDataDelete bigint=0,@commandcount nVARCHAR(MAX)
		declare @TblData table(Id bigint, supplierID nvarchar(100), itemID nvarchar(100), beginQty bigint, endQty bigint, 
		price float, FlagData nvarchar(10))

		if @JsonData !='null'
		begin
			insert into @TblData
			select * from OPENJSON(@JsonData) WITH(Id bigint, supplierID nvarchar(100), itemID nvarchar(100), beginQty bigint, endQty bigint, 
			price float, FlagData nvarchar(10))

			set @BykDataAdd=(select count(Id) from @TblData where FlagData='Add')
			set @BykDataEdit=(select count(Id) from @TblData where FlagData='Edit')
			set @BykDataDelete=(select count(Id) from @TblData where FlagData='Delete')

			if @BykDataAdd != 0
			begin
				--Validasi Exists data

				insert into M_PriceListVoucher(supplierID, itemID, beginQty, endQty, price, CreateDate, CreateUser)
				select supplierID, itemID, beginQty, endQty, price, getdate(), @UserId from @TblData where FlagData='Add'
			end

			if @BykDataEdit != 0
			begin
				--Validasi Exists data

				update a set a.UpdateDate=getdate(), a.UpdateUser=@UserId, a.supplierID=b.supplierID, a.itemID=b.itemID, 
				a.beginQty=b.beginQty, a.endQty=b.endQty, a.price=b.price
				from M_PriceListVoucher a inner join (select * from @TblData where FlagData='Edit') b
				on a.Id=b.Id
			end

			if @BykDataDelete != 0
			begin
				update a set a.DeletedDate=getdate(), a.DeletedUser=@UserId, a.IsDeleted=1
				from M_PriceListVoucher a inner join (select * from @TblData where FlagData='Delete') b
				on a.Id=b.Id
			end

			set @Msg=(
			select case when @BykDataDelete != 0 and @BykDataEdit != 0 and @BykDataAdd != 0 then 'Bulk'
			when @BykDataDelete != 0 and @BykDataAdd != 0 then 'Bulk'
			when @BykDataDelete != 0 and @BykDataEdit != 0 then 'Bulk'
			when @BykDataAdd != 0 and @BykDataEdit != 0 then 'Bulk'
			when @BykDataDelete != 0 then 'Delete'
			when @BykDataEdit != 0 then 'Update'
			when @BykDataAdd != 0 then 'Create' end)

			select 'Success '+ @Msg +' Action.' as Result;
		end
		else if @Field='GetPrice'
		begin
			select top 1 price from M_PriceListVoucher where IsDeleted=0 and itemID=@itemId and supplierID=@supplierID and
			@Qty between beginQty and endQty
		end
		else if @Field='GetById'
		begin
			select *
			,'Create by ' + CreateUser +  ' On : ' + CONVERT(varchar,CreateDate,121) as Creator
			,'Update by ' + UpdateUser +  ' On : ' + CONVERT(varchar,UpdateDate,121) as Updater
			from M_PriceListVoucher where IsDeleted=0 and Id=@KeyWord
		end
		else
		begin
			set @command=N'select *
			,''Create by '' + CreateUser +  '' On : '' + CONVERT(varchar,CreateDate,121) as Creator
			,''Update by '' + UpdateUser +  '' On : '' + CONVERT(varchar,UpdateDate,121) as Updater
			from M_PriceListVoucher where IsDeleted=0 '

			set @commandcount = N'select @Tot=count(IsDeleted) from M_PriceListVoucher where IsDeleted=0 '
			set @totalrecords = (select count(IsDeleted) from M_PriceListVoucher where IsDeleted=0)

			if @KeyHeader is not null
			begin
				set @commandfilter =N'and itemID like ''%'+ @KeyHeader +'%'' or supplierID like ''%'+ @KeyHeader +'%'' '
			end

			if @JsonFilter is not null
			begin
				set @wherecondition=(select dbo.FnJsonFilter(@JsonFilter))

				set @commandfilter = 'and ' + @wherecondition

			end

			if @PageNumber is not null
			begin
				if @PageSize='All' or @PageSize is null
				begin
				 set @PageSize = @totalrecords
				end

				set @command = @command + ISNULL(@commandfilter, '')
				+ ' ORDER BY Id OFFSET ' + CONVERT(VARCHAR, @PageNumber)
				+ ' ROWS FETCH NEXT ' + CONVERT(VARCHAR, @PageSize)
				+ ' ROWS ONLY  '

				set @commandcount=@commandcount + ISNULL(@commandfilter,'')
			end
			else
			begin
				set @command = @command + ISNULL(@commandfilter, '')
				set @commandcount=@commandcount + ISNULL(@commandfilter,'')
			end
						
			exec Sp_executesql @commandcount, N'@Tot nvarchar(100) OUTPUT',  @Tot=@totalrecordFilter OUTPUT;
			exec (@command)
			
			select @totalrecords as TotalRecord, @totalrecordFilter as TotalFilter
		end		
		
	end try
	BEGIN CATCH

        SELECT 'Error : ' + ERROR_MESSAGE() as Result;

    END CATCH;
END
GO
/****** Object:  StoredProcedure [dbo].[ProcCRUDSystemConfig]    Script Date: 17/11/2023 11:03:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Umar>
-- Create date: <13 November 2023>
-- Description:	<Process CRUD Master System Config>
-- =============================================
CREATE PROCEDURE [dbo].[ProcCRUDSystemConfig] 
	@Id varchar(100)=null, @Name varchar(256)=null, @SysCategory varchar(256)=null, @SysSubCategory varchar(256)=null, 
	@SysCode varchar(256)=null, @FlagData varchar(10)=null,@JSONData nvarchar(max)=null,
	@SysValue varchar(max)=null, @Description varchar(max)=null, @UserId VARCHAR(100) = NULL
AS
BEGIN
	SET NOCOUNT ON;

    begin try

	declare @TblData table(Id bigint, Name nvarchar(max), SystemCategory nvarchar(max), SystemSubCategory nvarchar(max), 
	SystemCode nvarchar(max), SystemValue nvarchar(max), Description nvarchar(max), FlagData nvarchar(10))
	declare @BykDataAdd bigint,@BykDataEdit bigint,@BykDataDelete bigint, @Msg nvarchar(10)='',
	@command nVARCHAR(MAX)='',@commandfilter nVARCHAR(MAX)= ''

	if @JSONData !=null
	begin
		insert into @TblData
		select * from openjson(@JSONData) with
		(
			Id bigint, Name nvarchar(max), SystemCategory nvarchar(max), SystemSubCategory nvarchar(max), SystemCode nvarchar(max), 
			SystemValue nvarchar(max), Description nvarchar(max), FlagData nvarchar(10)
		)

		set @BykDataAdd = (select count(FlagData) from @TblData where FlagData='Add')
		set @BykDataEdit = (select count(FlagData) from @TblData where FlagData='Edit')
		set @BykDataDelete = (select count(FlagData) from @TblData where FlagData='Delete')
	end

	if @FlagData='GetById'
	begin
		select *
		,'Create by ' + CreateUser +  ' On : ' + CONVERT(varchar,CreateDate,121) as Creator
		,'Update by ' + UpdateUser +  ' On : ' + CONVERT(varchar,UpdateDate,121) as Updater
		from M_SystemConfig where IsDeleted=0 and Id=@Id
	end	
	else if @FlagData='Bulk'
	begin
		if @BykDataAdd !=0
		begin
			insert into M_SystemConfig(Name, SystemCategory, SystemSubCategory, SystemCode, SystemValue, Description, CreateUser, CreateDate)
			select Name, SystemCategory, SystemSubCategory, SystemCode, SystemValue, Description, @UserId, getdate() 
			from @TblData where FlagData='Add'
		end

		if @BykDataEdit !=0
		begin
			update a set a.UpdateDate=getdate(), a.UpdateUser=@UserId, a.Name=b.Name, a.SystemCategory=b.SystemCategory, 
			a.SystemSubCategory=b.SystemSubCategory, a.SystemCode=b.SystemCode, a.SystemValue=b.SystemValue, a.Description=b.Description
			from M_SystemConfig a inner join (select * from @TblData where FlagData='Edit') b
			on a.Id=b.Id
		end

		if @BykDataDelete!=0
		begin
			update a set DeletedDate=getdate(),DeletedUser=@UserId, IsDeleted=1
			from M_SystemConfig a inner join (select Id, FlagData from @TblData where FlagData='Delete') b
			on a.Id=b.Id
		end
		
		set @Msg=(
			select case when @BykDataDelete != 0 and @BykDataEdit != 0 and @BykDataAdd != 0 then 'Bulk'
			when @BykDataDelete != 0 and @BykDataAdd != 0 then 'Bulk'
			when @BykDataDelete != 0 and @BykDataEdit != 0 then 'Bulk'
			when @BykDataAdd != 0 and @BykDataEdit != 0 then 'Bulk'
			when @BykDataDelete != 0 then 'Delete'
			when @BykDataEdit != 0 then 'Update'
			when @BykDataAdd != 0 then 'Create' end)

			select 'Success '+ @Msg +' Action.' as Result;
	end
	else
	begin
		set @command= N'select
		''Create by '' + CreateUser +  '' On : '' + CONVERT(varchar,CreateDate,121) as Creator
		,''Update by '' + UpdateUser +  '' On : '' + CONVERT(varchar,UpdateDate,121) as Updater
		,* from M_SystemConfig where IsDeleted=0 '

		if @Name !=null
		begin
			set @commandfilter=N'and Name='''+ @Name +''' '
		end
		if @SysCategory !=null
		begin
			set @commandfilter=N'and SystemCategory='''+ @SysCategory +''' '
		end
		if @SysSubCategory !=null
		begin
			set @commandfilter=N'and SystemSubCategory='''+ @SysSubCategory +''' '
		end
		if @SysCode !=null
		begin
			set @commandfilter=N'and SystemCode='''+ @SysCode +''' '
		end

		set @command = @command + ISNULL(@commandfilter, '')

		--print @command
		exec Sp_executesql @command
	end
	
	--else if @FlagData='Add'
	--begin
	--	insert into M_SystemConfig (Name, SystemCategory, SystemSubCategory, SystemCode, SystemValue, Description, CreateUser, CreateDate)
	--	values
	--	(@Name, @SysCategory, @SysSubCategory, @SysCode, @SysValue, @Description, @UserId, getdate())
	--	select 'Success Input System Config.' as Result
	--end
	--else if @FlagData='Edit'
	--begin
	--	update M_SystemConfig set Name=@Name, SystemCategory=@SysCategory, SystemSubCategory=@SysSubCategory, SystemCode=@SysCode, 
	--	SystemValue=@SysValue, Description=@Description, UpdateDate=getdate(), UpdateUser=@UserId where Id=@Id
	--	select 'Success Update System Config on '+ @SysSubCategory as Result
	--end
	--else if @FlagData='Delete'
	--begin
	--	if(@JSONData!='null')
	--	begin
	--		update M_SystemConfig set 
	--		DeletedDate=getdate(),DeletedUser=@UserId, IsDeleted=1 where Id in
	--		(
	--			select * from openjson(@JSONData) with
	--			(
	--				Id Int
	--			)
	--		)

	--		SELECT 'Success deleted Multiple System Config.' as Result;
	--	end
	--	else
	--	begin
	--		update M_SystemConfig set IsDeleted=1, DeletedDate=getdate(), DeletedUser=@UserId where Id=@Id
	--		select 'Success deleted System Config on '+ @SysSubCategory as Result
	--	end		
	--end
		
	end try
	BEGIN CATCH

        SELECT 'Error : ' + ERROR_MESSAGE() Result;

    END CATCH;
END
GO
/****** Object:  StoredProcedure [dbo].[ProcCRUDVoucherDetail]    Script Date: 17/11/2023 11:03:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Umar>
-- Create date: <9 Nov 2023>
-- Description:	<Proc CRUD Voucher Detail>
-- =============================================
CREATE PROCEDURE [dbo].[ProcCRUDVoucherDetail] 
	@UserId varchar(100)=null,
	@JsonData nvarchar(max)=null,
	@JsonFilter nvarchar(max)=null,
	@Field nvarchar(100)=null,
	@KeyWord nvarchar(max)=null,
	@KeyHeader nvarchar(max)=null,
	@PageNumber nvarchar(200)=null ,
    @PageSize nvarchar(200)=null ,
	@totalrecords INT OUTPUT,
	@totalrecordFilter INT OUTPUT
AS
BEGIN

	SET NOCOUNT ON;
	begin try
		declare @command nVARCHAR(MAX), @commandfilter nVARCHAR(MAX)= '', @Msg nvarchar(10)='', @wherecondition nvarchar(max)='',
		@BykDataAdd bigint=0, @BykDataEdit bigint=0, @BykDataDelete bigint=0, @commandcount nVARCHAR(MAX)
		declare @TblData table(Id bigint, refId nvarchar(50), itemID nvarchar(50), startNo nvarchar(50), endNo nvarchar(50), 
		expDateVoucher date, qty bigint, sources nvarchar(200), FlagData nvarchar(10))

		if @JsonData !='null'
		begin
			insert into @TblData
			select * from OPENJSON(@JsonData) WITH(Id bigint, refId nvarchar(50), itemID nvarchar(50), startNo nvarchar(50), 
			endNo nvarchar(50), expDateVoucher date, qty bigint, sources nvarchar(200), FlagData nvarchar(10))

			set @BykDataAdd=(select count(Id) from @TblData where FlagData='Add')
			set @BykDataEdit=(select count(Id) from @TblData where FlagData='Edit')
			set @BykDataDelete=(select count(Id) from @TblData where FlagData='Delete')

			if @BykDataAdd != 0
			begin
				insert into T_VoucherDetail(refId, itemID, startNo, endNo, expDateVoucher, qty, sources, CreateDate, CreateUser)
				select refId, itemID, startNo, endNo, expDateVoucher, qty, sources, getdate(), @UserId from @TblData where FlagData='Add'
			end
			if @BykDataEdit != 0
			begin
				update a set a.UpdateDate=getdate(), a.UpdateUser=@UserId, a.refId=b.refId, a.itemID=b.itemID, a.startNo=b.startNo, 
				a.endNo=b.endNo, a.expDateVoucher=b.expDateVoucher, a.qty=b.qty, a.sources=b.sources
				from T_VoucherDetail a inner join (select * from @TblData where FlagData='Edit') b
				on a.Id=b.Id
			end
			if @BykDataDelete != 0
			begin
				update a set a.DeletedDate=getdate(), a.DeletedUser=@UserId, a.IsDeleted=1
				from T_VoucherDetail a inner join (select * from @TblData where FlagData='Delete') b
				on a.Id=b.Id
			end

			set @Msg=(
			select case when @BykDataDelete != 0 and @BykDataEdit != 0 and @BykDataAdd != 0 then 'Bulk'
			when @BykDataDelete != 0 and @BykDataAdd != 0 then 'Bulk'
			when @BykDataDelete != 0 and @BykDataEdit != 0 then 'Bulk'
			when @BykDataAdd != 0 and @BykDataEdit != 0 then 'Bulk'
			when @BykDataDelete != 0 then 'Delete'
			when @BykDataEdit != 0 then 'Update'
			when @BykDataAdd != 0 then 'Create' end)

			select 'Success '+ @Msg +' Action.' as Result;
		end
		else if @Field='GetById'
		begin
			select *
			,FORMAT(expDateVoucher,'yyyy-MM-dd') as expDateVoucherDateUs
			,'Create by ' + CreateUser +  ' On : ' + CONVERT(varchar,CreateDate,121) as Creator
			,'Update by ' + UpdateUser +  ' On : ' + CONVERT(varchar,UpdateDate,121) as Updater
			from T_VoucherDetail where IsDeleted=0 and Id=@KeyWord
		end
		else
		begin
			set @command=N'select *
			,FORMAT(expDateVoucher,''yyyy-MM-dd'') as expDateVoucherDateUs
			,''Create by '' + CreateUser +  '' On : '' + CONVERT(varchar,CreateDate,121) as Creator
			,''Update by '' + UpdateUser +  '' On : '' + CONVERT(varchar,UpdateDate,121) as Updater
			from T_VoucherDetail where IsDeleted=0 '

			set @commandcount = N'select @Tot=count(IsDeleted) from T_VoucherDetail where IsDeleted=0 '
			set @totalrecords = (select count(IsDeleted) from T_VoucherDetail where IsDeleted=0)

			if @KeyHeader is not null
			begin
				set @commandfilter =N'and refId like ''%'+ @KeyHeader +'%'' or itemID like ''%'+ @KeyHeader +'%'' '
			end

			if @JsonFilter is not null
			begin
				set @wherecondition=(select dbo.FnJsonFilter(@JsonFilter))

				set @commandfilter = 'and ' + @wherecondition

			end

			if @PageNumber is not null
			begin
				if @PageSize='All' or @PageSize is null
				begin
				 set @PageSize = @totalrecords
				end

				set @command = @command + ISNULL(@commandfilter, '')
				+ ' ORDER BY Id OFFSET ' + CONVERT(VARCHAR, @PageNumber)
				+ ' ROWS FETCH NEXT ' + CONVERT(VARCHAR, @PageSize)
				+ ' ROWS ONLY  '

				set @commandcount=@commandcount + ISNULL(@commandfilter,'')
			end
			else
			begin
				set @command = @command + ISNULL(@commandfilter, '')
				set @commandcount=@commandcount + ISNULL(@commandfilter,'')
			end
						
			exec Sp_executesql @commandcount, N'@Tot nvarchar(100) OUTPUT',  @Tot=@totalrecordFilter OUTPUT;
			exec (@command)
			
			select @totalrecords as TotalRecord, @totalrecordFilter as TotalFilter
		end		
		
	end try
	BEGIN CATCH

        SELECT 'Error : ' + ERROR_MESSAGE() as Result;

    END CATCH;
END
GO
/****** Object:  StoredProcedure [dbo].[T_VoucherDetail_getlist]    Script Date: 17/11/2023 11:03:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[T_VoucherDetail_getlist] @parjson nvarchar(max)
as 
--declare @parjson nvarchar(max) = '[{"Code":"T_TransferStock","Field":null,"Keyword":null,"Status":"New","Username":"dev.andre"}]'
declare @sqlstr nvarchar(max)=''

/*declare param*/
declare @code nvarchar(50), @field varchar(50), @keyword varchar(200), @username varchar(50), @status varchar(50)
SELECT @code = code, @field = field, @keyword = keyword, @username = username, @status = status
--select *
FROM OPENJSON(@parjson, '$') 
WITH (
	Code VARCHAR(50),	Field VARCHAR(50),	Keyword VARCHAR(200),	Username varchar(50), [Status] varchar(50)
) AS a

if @code <> '' begin 
	set @sqlstr =
	
	'select 
		ROW_NUMBER() OVER(ORDER BY createdate ASC) [recid], a.*
	from '+@code+' a
	where 1=1 
	'+iif(@status <> '', 'and status = '''+@status+''' ', '')+'
	'+iif(@field <> '' and @keyword <> '', 'and '+@field+' = '''+@keyword+''' ', '')+'
	--'+iif(@status <> '', 'and createuser = '''+@username+''' ', '')+'
	'

	print @sqlstr
	exec (@sqlstr)
end
GO
/****** Object:  StoredProcedure [dbo].[T_VoucherDetail_save]    Script Date: 17/11/2023 11:03:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[T_VoucherDetail_save] @userlogin nvarchar(50), @refId nvarchar(50), @detaildata nvarchar(max)
as
--declare
--@userlogin nvarchar(50)='dev.andre', @refId nvarchar(50) = 'SL-GR-230100002', @detaildata nvarchar(max) = '[{"itemID":"00100537","qty":"10","startNo":"VMS2300001abcd","endNo":"VMS2300010abcd","ID":"0","recid":"1-Kz8h"}]'

delete from t_voucherdetail where refid = @refId
insert into t_voucherdetail (itemid, qty, startno, endno, expDateVoucher, refid, createuser, createdate, isdeleted)
select  itemid, qty, startno, endno, expDateVoucher, @refId, @userlogin, getdate(), 0
		FROM OPENJSON(@detaildata, '$') 
		WITH (
			itemID nvarchar(50),
			qty INT,startNo nvarchar(50), endNo nvarchar(50), expDateVoucher date
		) AS a

	select  'success' msg, @refId id
GO
