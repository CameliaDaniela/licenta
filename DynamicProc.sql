select  c.RoadSegment,avg( c.Speed) as SpeedSegment 
from Car c
group by c.RoadSegment
select MAX(TimeStmp)
FROM Car
select*from Car
where TimeStmp=1

DECLARE @sqlCommand nvarchar(1000)
DECLARE @column varchar(75)
DECLARE @tsmp int
DECLARE @IntVariable int
SET @column = 'RoadSegment'
SET @sqlCommand = 'SELECT ' + @column + ', AVG( Speed) FROM Car WHERE TimeStmp = @tsmp GROUP BY '+ @column
SET @IntVariable = 1;  
EXECUTE sp_executesql @sqlCommand, N'@tsmp int', @tsmp = @IntVariable
