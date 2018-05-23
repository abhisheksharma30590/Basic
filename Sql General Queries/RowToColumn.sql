SELECT REPLACE(ItemName,' ','') FROM [dbo].[Order]
DECLARE @Sql nvarchar(max)
SET @Sql  = 'CREATE TABLE temptable (' 
SELECT @Sql = @Sql + REPLACE(ItemName,' ','')+' int,'
FROM [dbo].[Order] 
SET @Sql = LEFT(@Sql, LEN(@Sql) - 1) +');'
SELECT @Sql
exec(@sql)
SELECT * FROM temptable