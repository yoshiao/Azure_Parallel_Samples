-- Calculate size in MB for each partition in the database

SELECT
      OBJECT_NAME(object_id) AS [Name],
      index_id AS [Index Id],
      row_count AS [Row Count],
      used_page_count * 8.0 / 1024 AS [Used in MB],
      reserved_page_count * 8.0 / 1024 as [Reserved in MB]
FROM  
      sys.dm_db_partition_stats
ORDER BY
	1
