-- Calculate size in MB for each object in database

SELECT
      OBJECT_NAME(object_id) AS [Name],
      SUM(reserved_page_count) * 8.0 / 1024 AS [Size in MB]
FROM  
      sys.dm_db_partition_stats
GROUP BY
	object_id
ORDER BY
	2 DESC
