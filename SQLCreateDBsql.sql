use [Licenta]
go

IF OBJECT_ID('dbo.Car') IS NOT NULL
    DROP TABLE dbo.Car;
go

IF OBJECT_ID('dbo.DataEvent') IS NOT NULL
    DROP TABLE dbo.DataEvent;
go
create table Car(
 IdCar int primary key identity(1,1),
 NameCar varchar(50) not null,
 Speed int
)
go
create table DataEvent(
IdDataEvent int primary key identity(1,1),
ActivityCode int ,
StartTime datetime,
StatusEvent varchar(50) not null
)
go

Create proc insertEventData
as 
begin
	Declare @id int,@actCode int,@statusEvent nvarchar(50),@startTime datetime
	DECLARE @ct int,@statusEventCode int,@randMonth int,@randYear int
	set @ct=0;
	while @ct<10
	begin
		
		set @startTime='2000-10-23 12:45:37' 
		set @ct=@ct+1
		set @actCode=RAND()*100+@ct;
		set @statusEventCode=RAND()*100
		if(@statusEventCode < 25)
			set @statusEvent ='Finished'
		else if(@statusEventCode < 50)
			set @statusEvent='In progress'
		else if(@statusEventCode <75)
			set @statusEvent ='Stopped'
		else	
			set @statusEvent='Pending'
		insert into DataEvent
		select @actCode, @startTime,@statusEvent
	end
	
end
go

use Licenta
exec insertEventData
drop proc insertEventData
go
select top 1 * from DataEvent
select * from Car
insert into DataEvent values(22,'1999-10-23 12:45:37' ,'Pending ')