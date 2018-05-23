DECLARE @TableName sysname
SET @TableName = 'ICODocuments'
DECLARE @Result VARCHAR(MAX)
DECLARE @entityVariable VARCHAR(50) ,
    @modelVariable VARCHAR(50) ,
    @seprationOperator VARCHAR(5)
SET @entityVariable = ''
SET @modelVariable = 'doc.'
SET @seprationOperator = ','

SET @Result = '{
 '

SELECT  @Result = @Result + '
    ' + @entityVariable + REPLACE(ColumnName, '_', '') + ' = '
        + @modelVariable + REPLACE(ColumnName, '_', '') + @seprationOperator
        + ''
FROM    ( SELECT    REPLACE(col.name, ' ', '_') ColumnName ,
                    column_id ColumnId
          FROM      sys.columns col
          WHERE     object_id = OBJECT_ID(@TableName)
        ) t
ORDER BY ColumnId

SET @Result = @Result + '
}'

PRINT @Result