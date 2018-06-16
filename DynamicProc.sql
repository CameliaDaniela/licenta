select  c.RoadSegment,avg( c.Speed) as SpeedSegment 
from Car c
group by  c.TimeStmp,c.RoadSegment
select MAX(TimeStmp)
FROM Car
select*from Car
where TimeStmp=1

DECLARE @sqlCommand nvarchar(1000)
DECLARE @column varchar(75)
DECLARE @tsmp int
DECLARE @IntVariable int
DECLARE @whereClause varchar(100)
SET @column = 'RoadSegment'
set @whereClause =''
SET @sqlCommand = 'SELECT ' + @column + ', AVG( Speed) FROM Car '+ @whereClause +' GROUP BY '+ @column
SET @IntVariable = 1;  
EXECUTE sp_executesql @sqlCommand, N'@tsmp int', @tsmp = @IntVariable

select  c.GroupId,avg( c.Avreage) as SpeedSegment 
from Query c
group by c.GroupId
