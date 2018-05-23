DECLARE @TableName sysname
SET @TableName = 'CommunicationDetail'
DECLARE @ProcedureName sysname
SET @ProcedureName = 'Add_EmployeeMaster'
DECLARE @Result VARCHAR(MAX)
SET @Result = 'ALTER PROCEDURE ' + @ProcedureName + '
('
SELECT  @Result = @Result + '
   ' + CASE WHEN X.Row = 1
            THEN REPLACE(LEFT(X.Parameters, LEN(X.Parameters) - 1), '_', '')
            ELSE REPLACE(X.Parameters, '_', '')
       END + ''
FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY ORDINAL_POSITION DESC ) AS Row ,
                    '@' + LOWER(LEFT(COLUMN_NAME, 1)) + SUBSTRING(COLUMN_NAME,
                                                              2,
                                                              LEN(COLUMN_NAME))
                    + '  ' + DATA_TYPE + CASE DATA_TYPE
                                           WHEN 'bigint' THEN ','
                                           WHEN 'binary' THEN ','
                                           WHEN 'bit' THEN ','
                                           WHEN 'char'
                                           THEN '('
                                                + CAST(CHARACTER_MAXIMUM_LENGTH AS NVARCHAR(50))
                                                + '),'
                                           WHEN 'date' THEN ','
                                           WHEN 'datetime' THEN ','
                                           WHEN 'datetime2' THEN ','
                                           WHEN 'datetimeoffset' THEN ','
                                           WHEN 'decimal'
                                           THEN '('
                                                + CAST(NUMERIC_PRECISION AS NVARCHAR(50))
                                                + ','
                                                + CAST(NUMERIC_SCALE AS NVARCHAR(50))
                                                + '),'
                                           WHEN 'float' THEN ','
                                           WHEN 'image' THEN ','
                                           WHEN 'int' THEN ','
                                           WHEN 'money' THEN ','
                                           WHEN 'nchar'
                                           THEN '('
                                                + CAST(CHARACTER_MAXIMUM_LENGTH AS NVARCHAR(50))
                                                + '),'
                                           WHEN 'ntext' THEN ','
                                           WHEN 'numeric'
                                           THEN '('
                                                + CAST(NUMERIC_PRECISION AS NVARCHAR(50))
                                                + ','
                                                + CAST(NUMERIC_SCALE AS NVARCHAR(50))
                                                + '),'
                                           WHEN 'nvarchar'
                                           THEN '('
                                                + CASE WHEN CHARACTER_MAXIMUM_LENGTH = -1
                                                       THEN 'MAX'
                                                       ELSE CAST(CHARACTER_MAXIMUM_LENGTH AS NVARCHAR(50))
                                                  END + '),'
                                           WHEN 'real' THEN ','
                                           WHEN 'smalldatetime' THEN ','
                                           WHEN 'smallint' THEN ','
                                           WHEN 'smallmoney' THEN ','
                                           WHEN 'text' THEN ','
                                           WHEN 'time' THEN ','
                                           WHEN 'timestamp' THEN ','
                                           WHEN 'tinyint' THEN ','
                                           WHEN 'uniqueidentifier' THEN ','
                                           WHEN 'varbinary' THEN ','
                                           WHEN 'varchar'
                                           THEN '('
                                                + CASE WHEN CHARACTER_MAXIMUM_LENGTH = -1
                                                       THEN 'MAX'
                                                       ELSE CAST(CHARACTER_MAXIMUM_LENGTH AS NVARCHAR(50))
                                                  END + '),'
                                           ELSE ','
                                         END AS Parameters
          FROM      INFORMATION_SCHEMA.COLUMNS
          WHERE     TABLE_NAME = @TableName
        ) X
ORDER BY X.Row DESC
SET @Result = @Result + '
)
As
BEGIN
END'
PRINT @Result

--SELECT  '@' + LOWER(LEFT(COLUMN_NAME, 1)) + SUBSTRING(COLUMN_NAME, 2,
--                                                      LEN(COLUMN_NAME)) AS Parameters
--FROM    INFORMATION_SCHEMA.COLUMNS
--WHERE   TABLE_NAME = @TableName