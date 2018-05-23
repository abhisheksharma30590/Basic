
DECLARE @CommentToAdd VARCHAR(255)
DECLARE @VariableTextOption VARCHAR(255)
DECLARE @SQLString NVARCHAR(MAX)

SET @CommentToAdd = '/* -- ============================================='
    + CHAR(13) + '--Author:		<Abhishek Sharma> ' + CHAR(13)
    + '--Create date: <30/12/16>' + CHAR(13) + '--Description:	<' 
SET @VariableTextOption = CHAR(13) + '-- ============================================= */'
    + CHAR(13) 

DECLARE @spname VARCHAR(100)
DECLARE db_cursor CURSOR FOR  


SELECT  t.sp_name
from
(
    SELECT DISTINCT
            o.name AS sp_name ,
			RANK() OVER	(ORDER BY o.name) rown
    FROM    sysobjects o
            INNER JOIN syscomments c ON c.id = o.id
    WHERE   o.xtype IN ( 'P' )
            AND o.category = 0
            AND o.name NOT IN ( 'fn_diagramobjects', 'sp_alterdiagram',
                                'sp_creatediagram', 'sp_dropdiagram',
                                'sp_helpdiagramdefinition', 'sp_helpdiagrams',
                                'sp_renamediagram', 'sp_upgraddiagrams',
                                'sysdiagrams' )
								
) t
WHERE t.rown > 65
order by 1

OPEN db_cursor   
FETCH NEXT FROM db_cursor INTO @spname  

WHILE @@FETCH_STATUS = 0   
BEGIN   
      
SET @SQLString = ( SELECT   @CommentToAdd + ROUTINE_NAME + '>'
                            + @VariableTextOption + 'ALTER '
                            + RIGHT(r.ROUTINE_DEFINITION,
                                    ( LEN(r.ROUTINE_DEFINITION)
                                      - CHARINDEX('PROCEDURE',
                                                  r.ROUTINE_DEFINITION, 0) + 1 ))
                   FROM     INFORMATION_SCHEMA.ROUTINES r
                   WHERE    r.ROUTINE_TYPE = 'PROCEDURE'
                            AND ROUTINE_NAME = @spname
                 ) 
				 
		EXEC (@SQLString)
       FETCH NEXT FROM db_cursor INTO @spname   
END   

CLOSE db_cursor   
DEALLOCATE db_cursor 