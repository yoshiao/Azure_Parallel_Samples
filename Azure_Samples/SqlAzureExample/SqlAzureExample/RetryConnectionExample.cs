using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.SqlClient;
using Microsoft.AppFabricCAT.Samples.Azure.TransientFaultHandling;
using Microsoft.AppFabricCAT.Samples.Azure.TransientFaultHandling.SqlAzure;
using Microsoft.AppFabricCAT.Samples.Azure.TransientFaultHandling.Configuration;

namespace SqlAzureExample
{
    class RetryConnectionExample
    {
        String connectionString;
        RetryPolicy connectionRetryPolicy;
        RetryPolicy commandRetryPolicy;

        public RetryConnectionExample(String server, String database, String login, String password)
        {
            SqlConnectionStringBuilder connStringBuilder;
            connStringBuilder = new SqlConnectionStringBuilder();
            connStringBuilder.DataSource = String.Format("{0}.database.windows.net", server);
            connStringBuilder.InitialCatalog = database;
            connStringBuilder.Encrypt = true;
            connStringBuilder.TrustServerCertificate = false;
            connStringBuilder.UserID = String.Format("{0}@{1}", login, server);
            connStringBuilder.Password = password;
            connectionString = connStringBuilder.ToString();

            connectionRetryPolicy = new RetryPolicy<SqlAzureTransientErrorDetectionStrategy>(5, TimeSpan.FromMilliseconds(100));
            connectionRetryPolicy.RetryOccurred += RetryConnectionCallback;

            RetryPolicyConfigurationSettings retryPolicySettings = ApplicationConfiguration.Current.GetConfigurationSection<RetryPolicyConfigurationSettings>(RetryPolicyConfigurationSettings.SectionName);
            RetryPolicyInfo retryPolicyInfo = retryPolicySettings.Policies.Get("FixedIntervalDefault");
            commandRetryPolicy = retryPolicyInfo.CreatePolicy<SqlAzureTransientErrorDetectionStrategy>();
            commandRetryPolicy.RetryOccurred += RetryCallbackCommand;
        }

        private void RetryConnectionCallback(Int32 currentRetryCount, Exception lastException, TimeSpan delay)
        {
            Int32 retryCount = currentRetryCount;
        }

        private void RetryCallbackCommand(Int32 currentRetryCount, Exception lastException, TimeSpan delay)
        {
            Int32 retryCount = currentRetryCount;
        }

        public String GetSessionTracingId()
        {
            // using all default policies
            String commandText = "SELECT CONVERT(NVARCHAR(36), CONTEXT_INFO())";
            String sessionTracingId;
            using (ReliableSqlConnection connection = new ReliableSqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand sqlCommand = connection.CreateCommand())
                {
                    sqlCommand.CommandText = commandText;
                    sessionTracingId = sqlCommand.ExecuteScalarWithRetry() as String;
                }
            }
            return sessionTracingId;
        }

        public void CreateTable()
        {
            // Uses specified connection for open, default for execute
            String commandText =
              @"CREATE TABLE Writer (
                  Id int PRIMARY KEY NOT NULL,
                  Name nvarchar(20) NOT NULL,
                  CountBooks int NULL)";
            using (ReliableSqlConnection connection = new ReliableSqlConnection(connectionString, connectionRetryPolicy))
            {
                connection.Open();
                using (SqlCommand sqlCommand = connection.CreateCommand())
                {
                    sqlCommand.CommandText = commandText;
                    sqlCommand.ExecuteNonQueryWithRetry();
                }
            }
        }

        public void DropTable()
        {
            // Uses default for connection open and specified for command
            String commandText = "DROP TABLE Writer";
            using (ReliableSqlConnection connection = new ReliableSqlConnection(connectionString))
            {
                connection.Open(connectionRetryPolicy);
                using (SqlCommand sqlCommand = connection.CreateCommand())
                {
                    sqlCommand.CommandText = commandText;
                    sqlCommand.ExecuteNonQueryWithRetry(commandRetryPolicy);
                }
            }
        }

        public void QueryTable()
        {
            //uses provided policies
            String commandText = "SELECT * FROM Writer";
            using (ReliableSqlConnection connection = new ReliableSqlConnection(connectionString, connectionRetryPolicy, commandRetryPolicy))
            {
                connection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(commandText, connection.Current))
                {
                    using (IDataReader reader = connection.ExecuteCommand<IDataReader>(sqlCommand))
                    {
                        Int32 idColumn = reader.GetOrdinal("Id");
                        Int32 nameColumn = reader.GetOrdinal("Name");
                        Int32 countBooksColumn = reader.GetOrdinal("CountBooks");
                        while (reader.Read())
                        {
                            Int32 id = (Int32)reader[idColumn];
                            String name = reader[nameColumn] as String;
                            Int32? countBooks = reader[countBooksColumn] as Int32?;
                        }
                    }
                }
            }
        }

        public Int32 InsertRows()
        {
            // uses default retry policies
            String commandText =
              @"INSERT INTO Writer
                (Id, Name, CountBooks)
                VALUES
                  (1, N'Cervantes', 2),
                  (2, N'Smollett', null),
                  (3, 'Beyle', 4)";
            Int32 rowsAffected;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.OpenWithRetry();
                using (SqlCommand sqlCommand = new SqlCommand(commandText, connection))
                {
                    rowsAffected = sqlCommand.ExecuteNonQueryWithRetry();
                }
            }
            return rowsAffected;
        }

        public Int32 UpdateRow()
        {
            //uses default exponential RetyPolicy

            RetryPolicy exponentialRetryPolicy = RetryPolicy.DefaultExponential;

            String commandText =
               @"UPDATE Writer
                 SET Name=@Name
                 WHERE Id=3";
            Int32 rowsAffected;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.OpenWithRetry(exponentialRetryPolicy);
                using (SqlCommand sqlCommand = new SqlCommand(commandText, connection))
                {
                    SqlParameter sqlParameter = new SqlParameter()
                    {
                        ParameterName = "@Name",
                        Value = "Stendhal",
                        SqlDbType = SqlDbType.NVarChar,
                        Size = 20
                    };
                    sqlCommand.Parameters.Add(sqlParameter);
                    rowsAffected = sqlCommand.ExecuteNonQueryWithRetry(exponentialRetryPolicy);
                }
            }
            return rowsAffected;
        }

        public static void UseRetryConnectionExample()
        {
            String server = "SERVER_NAME";
            String database = "DATABASE_NAME";
            String login = "LOGIN";
            String password = "PASSWORD";

            RetryConnectionExample example = new RetryConnectionExample(server, database, login, password);
            example.GetSessionTracingId();
            example.CreateTable();
            example.InsertRows();
            example.QueryTable();
            example.UpdateRow();
            example.QueryTable();
            example.DropTable();
        }
    }
}
