DECLARE @Table sysname
SET @Table = 'Transaction_Auction_Fees'
DECLARE @TableName sysname
DECLARE @variableName VARCHAR(50)
SET @TableName = REPLACE(@Table, '_', '')
SET @variableName = LOWER(LEFT(@TableName, 1)) + SUBSTRING(@TableName, 2,
                                                           LEN(@TableName))
DECLARE @Result VARCHAR(MAX) 
SET @Result = 'public class ' + REPLACE(@TableName, '_', '')
    + 'Mapper : AbstractMapper<' + @TableName + '>
{'


SET @Result = @Result + '
    public ' + @TableName + ' MapEntity(IDataReader reader)
    {'
SET @Result = @Result + '
      ' + @TableName + ' _' + @variableName + ' = new ' + @TableName + '();
'
SET @Result = @Result + '      while (reader.Read())
      {'
SET @Result = @Result + '
        _' + @variableName + ' = MapReader(reader);
      }'
SET @Result = @Result + '
        return _' + @variableName + ';
     }
	 '


SET @Result = @Result + '
     public List<' + @TableName + '> MapEntityCollection(IDataReader reader)
     {'
SET @Result = @Result + '
       List<' + @TableName + '> _' + @variableName + 'list = new List<'
    + @TableName + '>();
'
SET @Result = @Result + '       while (reader.Read())
      {'
SET @Result = @Result + '
        _' + @variableName + 'list.Add(MapReader(reader));
      }'
SET @Result = @Result + '
       return _' + @variableName + 'list;
      }
	  '


SET @Result = @Result + '
     public ' + @TableName + ' MapReader(IDataReader reader, Dictionary<string, int> columnNameAndIndex = null)
     {'
SET @Result = @Result + '
       ' + @TableName + ' _' + @variableName + ' = new ' + @TableName + '
		{'

DECLARE @mapper VARCHAR(MAX) 
SET @mapper = ''
SELECT  @mapper = @mapper + '
			' + REPLACE(t.ColumnName, '_', '') + ' =DataReaderHelper.'
        + t.ColumnType + '(reader,"' + ColumnName + '",columnNameAndIndex),'
FROM    ( SELECT    REPLACE(col.name, ' ', '_') ColumnName ,
                    column_id ColumnId ,
                    CASE typ.name
                      WHEN 'bigint' THEN 'GetInt64Value'
                      WHEN 'binary' THEN 'GetStringValue'
                      WHEN 'bit' THEN 'GetBoolValue'
                      WHEN 'char' THEN 'GetStringValue'
                      WHEN 'date' THEN 'GetDateTimeValue'
                      WHEN 'datetime' THEN 'GetDateTimeValue'
                      WHEN 'datetime2' THEN 'GetDateTimeValue'
                      WHEN 'datetimeoffset' THEN 'GetDateTimeValue'
                      WHEN 'decimal' THEN 'GetDecimalValue'
                      WHEN 'float' THEN 'GetDecimalValue'
                      WHEN 'image' THEN 'GetStringValue'
                      WHEN 'int' THEN 'GetInt32Value'
                      WHEN 'money' THEN 'GetDecimalValue'
                      WHEN 'nchar' THEN 'GetCharsValue'
                      WHEN 'ntext' THEN 'GetStringValue'
                      WHEN 'numeric' THEN 'GetDecimalValue'
                      WHEN 'nvarchar' THEN 'GetStringValue'
                      WHEN 'real' THEN 'GetInt64Value'
                      WHEN 'smalldatetime' THEN 'GetDateTimeValue'
                      WHEN 'smallint' THEN 'GetInt16Value'
                      WHEN 'smallmoney' THEN 'GetDecimalValue'
                      WHEN 'text' THEN 'GetStringValue'
                      WHEN 'time' THEN 'GetDecimalValue'
                      WHEN 'timestamp' THEN 'GetDecimalValue'
                      WHEN 'tinyint' THEN 'GetStringValue'
                      WHEN 'uniqueidentifier' THEN 'GetStringValue'
                      WHEN 'varbinary' THEN 'GetStringValue'
                      WHEN 'varchar' THEN 'GetStringValue'
                      ELSE 'UNKNOWN_' + typ.name
                    END ColumnType ,
                    CASE WHEN col.is_nullable = 1
                              AND typ.name IN ( 'bigint', 'bit', 'date',
                                                'datetime', 'datetime2',
                                                'datetimeoffset', 'decimal',
                                                'float', 'int', 'money',
                                                'numeric', 'real',
                                                'smalldatetime', 'smallint',
                                                'smallmoney', 'time',
                                                'tinyint', 'uniqueidentifier' )
                         THEN '?'
                         ELSE ''
                    END NullableSign
          FROM      sys.columns col
                    JOIN sys.types typ ON col.system_type_id = typ.system_type_id
                                          AND col.user_type_id = typ.user_type_id
          WHERE     object_id = OBJECT_ID(@Table)
        ) t
ORDER BY ColumnId

SET @Result = @Result + LEFT(@mapper, LEN(@mapper) - 1) + '
		 };' + '
		return _' + @variableName + ';
	   }
	   '


SET @Result = @Result + '
}'

PRINT @Result