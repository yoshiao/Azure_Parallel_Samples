-- Calculate the size of the database in MB

SELECT
	DB_NAME() AS [Database],
	SUM(reserved_page_count) * 8.0 /1024 AS [Size in MB]
FROM
	sys.dm_db_partition_stats; 
