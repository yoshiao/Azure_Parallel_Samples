-- Find information about current connections

SELECT
	ec.connection_id,
	es.session_id,
	es.login_name,
	es.last_request_end_time,
	es.cpu_time
FROM
	sys.dm_exec_sessions AS [es]
INNER JOIN
	sys.dm_exec_connections AS [ec]
ON
	es.session_id = ec.session_id
