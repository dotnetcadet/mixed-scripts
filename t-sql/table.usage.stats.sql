Select 
    OBJECT_SCHEMA_NAME(UStat.object_id) As [ScheamName],
    OBJECT_NAME(UStat.object_id) As [TableName] 
    ,Case
        When Sum(User_Updates + User_Seeks + User_Scans + User_Lookups) = 0 Then Null
        Else Cast(Sum(User_Seeks + User_Scans + User_Lookups) As Decimal)
                    / Cast(Sum(User_Updates 
                                + User_Seeks 
                                + User_Scans
                                + User_Lookups) As Decimal(19,2))
        End As [ReadProportion] 
    , Case
        When Sum(User_Updates + User_Seeks + User_Scans + User_Lookups) = 0 Then Null
        Else Cast(Sum(User_Updates) As Decimal)
                / Cast(Sum(User_Updates + User_Seeks + User_Scans + User_Lookups) As Decimal(19,2))
        End As [WritesProportion] 
    , Sum(User_Seeks + User_Scans + User_Lookups) As [ReadTotal] 
    , Sum(User_Updates) As [WriteTotals]
From sys.dm_db_Index_Usage_Stats As UStat Inner Join Sys.Indexes As I 
On UStat.object_id = I.object_id And UStat.index_Id = I.index_Id Inner Join sys.tables As T
        On T.object_id = UStat.object_id
Where I.Type_Desc In ( 'Clustered', 'Heap' )
Group By UStat.object_id
Order By object_schema_name(UStat.object_id) 
        + '.' + object_name(UStat.object_id)