USE FinFusion
GO
SELECT file_id, name as [logical_file_name],physical_name
from sys.database_files


USE DSFinFusion
GO
SELECT file_id, name as [logical_file_name],physical_name
from sys.database_files


USE [master];
GO
ALTER DATABASE [Manvendra] MODIFY FILE ( NAME = Manvendra_Data, NEWNAME = Manvendra );
GO
ALTER DATABASE [Manvendra] MODIFY FILE ( NAME = Manvendra_1, NEWNAME = Manvendra_Data1 );
GO
ALTER DATABASE [Manvendra] MODIFY FILE ( NAME = Manvendra_2, NEWNAME = Manvendra_Data2 );