Select
        [TABLE_SCHEMA] As [Schema],
        [TABLE_NAME] As [Table],
        [COLUMN_NAME] As [OrginalColumn],
        REPLACE(CONCAT(UPPER(SUBSTRING([COLUMN_NAME],1,1)), SUBSTRING([COLUMN_NAME],2,LEN([COLUMN_NAME])-1)),'_','') As [TransformedColumnName],
        Case DATA_TYPE
            When  'varchar' Then IIF(IS_NULLABLE = 'YES', 'string?', 'string')
            When  'nvarchar' Then IIF(IS_NULLABLE = 'YES', 'string?', 'string')
            When  'char' Then IIF(IS_NULLABLE = 'YES', 'string?', 'string')
            When  'nchar' Then IIF(IS_NULLABLE = 'YES', 'string?', 'string')
            When  'text' Then IIF(IS_NULLABLE = 'YES', 'string?', 'string')
            When  'ntext' Then IIF(IS_NULLABLE = 'YES', 'string?', 'string')
            When  'int' Then IIF(IS_NULLABLE = 'YES', 'long?', 'long')
            When  'bigint' Then IIF(IS_NULLABLE = 'YES', 'Int64?', 'Int64')
            When  'bit' Then IIF(IS_NULLABLE = 'YES', 'bool?', 'bool')
            When  'smallint' Then IIF(IS_NULLABLE = 'YES', 'int?', 'int')
            When  'tinyint' Then IIF(IS_NULLABLE = 'YES', 'byte?', 'byte')
            When  'float' Then IIF(IS_NULLABLE = 'YES', 'double?', 'double')
            When  'real' Then IIF(IS_NULLABLE = 'YES', 'Single?', 'Single')
            When  'decimal' Then IIF(IS_NULLABLE = 'YES', 'decimal?', 'decimal')
            When  'numeric' Then IIF(IS_NULLABLE = 'YES', 'Single?', 'Single')
            When  'uniqueidentifier' Then IIF(IS_NULLABLE = 'YES', 'Guid?', 'Guid')
            When  'date' Then IIF(IS_NULLABLE = 'YES', 'DateTime?', 'DateTime')
            When  'datetime' Then IIF(IS_NULLABLE = 'YES', 'DateTime?', 'DateTime')
            When  'datetime2' Then IIF(IS_NULLABLE = 'YES', 'DateTime?', 'DateTime')
            When  'datetimeoffset' Then IIF(IS_NULLABLE = 'YES', 'DateTime?', 'DateTime')
            When  'smalldatetime' Then IIF(IS_NULLABLE = 'YES', 'DateTime?', 'DateTime')
            When  'timestamp' Then IIF(IS_NULLABLE = 'YES', 'byte[]?', 'byte[]')
            When  'money' Then IIF(IS_NULLABLE = 'YES', 'decimal?', 'decimal')
        End As [PropertyType],
        Case 
            When ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'varchar' Then '= string.Empty;'
            When ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'nvarchar' Then '= string.Empty;'
            When ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'char' Then '= string.Empty;'
            When ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'nchar' Then '= string.Empty;'
            When ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'text' Then '= string.Empty;'
            When ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'ntext' Then '= string.Empty;'
            when ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'int' Then ' = ' + SUBSTRING(COLUMN_DEFAULT,3,LEN(COLUMN_DEFAULT)-4) + ';'
            when ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'float' Then ' = ' + SUBSTRING(COLUMN_DEFAULT,3,LEN(COLUMN_DEFAULT)-4) + ';'
            when ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'real' Then ' = ' + SUBSTRING(COLUMN_DEFAULT,3,LEN(COLUMN_DEFAULT)-4) + ';'
            when ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'decimal' Then ' = ' + SUBSTRING(COLUMN_DEFAULT,3,LEN(COLUMN_DEFAULT)-4) + ';'
            when ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'numeric' Then ' = ' + SUBSTRING(COLUMN_DEFAULT,3,LEN(COLUMN_DEFAULT)-4) + ';'
            when ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'bigint' Then ' = ' + SUBSTRING(COLUMN_DEFAULT,3,len(COLUMN_DEFAULT)-4) + ';'
            when ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'money' Then ' = ' + SUBSTRING(COLUMN_DEFAULT,3,len(COLUMN_DEFAULT)-4) + ';'
            when ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'bit' Then ' = ' + IIF(SUBSTRING(COLUMN_DEFAULT,3,1)='1','true','false') + ';'
            when ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'uniqueidentifier' Then ' = new Guid();'
            when ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'datetime' Then ' = DateTime.Now;'
            when ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'datetime2' Then ' = DateTime.Now;'
            when ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'datetimeoffset' Then ' = DateTime.Now;'
            when ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'smalldatetime' Then ' = DateTime.Now;'
            when ISNULL(COLUMN_DEFAULT,'') <> '' and DATA_TYPE = 'time' Then ' = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);'
        End As [DefaultPropertyType],


        Case DATA_TYPE
            When  'varchar' Then IIF(CHARACTER_MAXIMUM_LENGTH=-1,'Varchar(Max)', CONCAT('Varchar(',CONVERT(varchar,CHARACTER_MAXIMUM_LENGTH),')'))
            When  'nvarchar' Then IIF(CHARACTER_MAXIMUM_LENGTH=-1,'Nvarchar(Max)', CONCAT('Nvarchar(',CONVERT(varchar,CHARACTER_MAXIMUM_LENGTH),')'))
            When  'char' Then IIF(CHARACTER_MAXIMUM_LENGTH=-1,'char(Max)', CONCAT('char(',CONVERT(varchar,CHARACTER_MAXIMUM_LENGTH),')'))
            When  'nchar' Then IIF(CHARACTER_MAXIMUM_LENGTH=-1,'Nchar(Max)', CONCAT('Nchar(',CONVERT(varchar,CHARACTER_MAXIMUM_LENGTH),')'))
            When  'text' Then 'text'
            When  'ntext' Then 'ntext'
            When  'int' Then 'Int'
            When  'bigint' Then 'BigInt'
            When  'bit' Then 'Bit'
            When  'smallint' Then 'SmallInt'
            When  'tinyint' Then 'TinyInt'
            When  'float' Then CONCAT('Float(',Convert(Varchar,NUMERIC_PRECISION),')')
            When  'real' Then IIF(IS_NULLABLE = 'YES', 'Single?', 'Single')
            When  'decimal' Then CONCAT('Decimal(',Convert(Varchar,NUMERIC_PRECISION), Convert(Varchar,NUMERIC_PRECISION_RADIX),')')
            When  'numeric' Then CONCAT('Numeric(',Convert(Varchar,NUMERIC_PRECISION), Convert(Varchar,NUMERIC_PRECISION_RADIX),')')
            When  'uniqueidentifier' Then 'Uniqueidentifier'
            When  'date' Then 'Date'
            When  'datetime' Then 'DateTime'
            When  'datetime2' Then CONCAT('DateTime2(',CONVERT(varchar,DATETIME_PRECISION),')')
            When  'datetimeoffset' Then CONCAT('DateTimeOffset(',CONVERT(varchar,DATETIME_PRECISION),')')
            When  'smalldatetime' Then CONCAT('SmallDateTime(',CONVERT(varchar,DATETIME_PRECISION),')')
            When  'timestamp' Then 'TimeStamp'
            When  'money' Then 'Money'
        End As [ColumnAttributeType],
        IIF(ISNULL([PKeys].[Column],'')<>'','[Key]',NULL) As [KeyAttribute]
From ['$(Database)'].INFORMATION_SCHEMA.COLUMNS Col Left Join 
	(
		Select
			SCHEMA_NAME(tab.schema_id) As [Schema], 
			pk.[name] As [PrimaryKey],
			col.[name] As [Column], 
			tab.[name] As [Table]
		From 
			['$(Database)'].sys.tables tab Inner Join ['$(Database)'].sys.indexes pk
		On tab.object_id = pk.object_id  And pk.is_primary_key = 1 Inner Join  ['$(Database)'].sys.index_columns ic
		On ic.object_id = pk.object_id And ic.index_id = pk.index_id Inner Join ['$(Database)'].sys.columns col
		On pk.object_id = col.object_id And col.column_id = ic.column_id	
	) As PKeys
On Col.TABLE_SCHEMA = PKeys.[Schema] And Col.TABLE_NAME = PKeys.[Table] And Col.COLUMN_NAME = [PKeys].[Column] 
Where Col.TABLE_SCHEMA = '$(Schema)' And Col.TABLE_NAME = '$(Table)'