-- Find top 5 queries by average worker time

SELECT TOP 5
	query_stats.query_hash AS [Query Hash],
	SUM(query_stats.total_worker_time) / ( 1000000.0 * SUM(query_stats.execution_count)) AS [Avg CPU Time (s)],
	SUM(query_stats.total_logical_reads) / ( 1000000.0 * SUM(query_stats.execution_count)) AS [Avg Logical Reads (s)],
	SUM(query_stats.total_logical_writes) / ( 1000000.0 * SUM(query_stats.execution_count)) AS [Avg Logical Writes (s)],
	MIN(query_stats.statement_text) AS [Statement Text]
FROM 
	(SELECT
		qs.*, 
		SUBSTRING(ST.text, (qs.statement_start_offset/2) + 1,
		((CASE statement_end_offset 
		WHEN -1 THEN DATALENGTH(st.text)
		ELSE qs.statement_end_offset END 
		- qs.statement_start_offset)/2) + 1) AS [statement_text]
	FROM
		sys.dm_exec_query_stats AS [qs]
	CROSS APPLY
		sys.dm_exec_sql_text(qs.sql_handle) AS [st]) AS [query_stats]
GROUP BY
	query_stats.query_hash
ORDER BY
	2 DESC