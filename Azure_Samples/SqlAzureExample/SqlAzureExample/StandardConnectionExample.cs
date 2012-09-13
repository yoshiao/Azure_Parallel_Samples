using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Data;

namespace SqlAzureExample
{
    class StandardConnectionExample
    {
        String connectionString;

        public StandardConnectionExample(String server, String database, String login, String password)
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
        }

        public String GetSessionTracingId()
        {
            String commandText = "SELECT CONVERT(NVARCHAR(36), CONTEXT_INFO())";
            String sessionTracingId;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand sqlCommand = connection.CreateCommand())
                {
                    sqlCommand.CommandText = commandText;
                    sessionTracingId = sqlCommand.ExecuteScalar() as String;
                }
            }
            return sessionTracingId;
        }

        public void CreateTable()
        {
            String commandText =
              @"CREATE TABLE Writer (
                  Id int PRIMARY KEY NOT NULL,
                  Name nvarchar(20) NOT NULL,
                  CountBooks int NULL)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand sqlCommand = connection.CreateCommand())
                {
                    sqlCommand.CommandText = commandText;
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public void DropTable()
        {
            String commandText = "DROP TABLE Writer";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand sqlCommand = connection.CreateCommand())
                {
                    sqlCommand.CommandText = commandText;
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public void QueryTable()
        {
            String commandText = "SELECT * FROM Writer";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(commandText, connection))
                {
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
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
                connection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(commandText, connection))
                {
                    rowsAffected = sqlCommand.ExecuteNonQuery();
                }
            }
            return rowsAffected;
        }

        public Int32 UpdateRow()
        {
            String commandText =
               @"UPDATE Writer
                 SET Name=@Name
                 WHERE Id=3";
            Int32 rowsAffected;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
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
                    rowsAffected = sqlCommand.ExecuteNonQuery();
                }
            }
            return rowsAffected;
        }

        public static void UseStandardConnectionExample()
        {
            String server = "SERVER_NAME";
            String database = "DATABASE_NAME";
            String login = "LOGIN";
            String password = "PASSWORD";

            StandardConnectionExample example = new StandardConnectionExample( server, database, login, password);
            String sessionTracingId = example.GetSessionTracingId();

            example.CreateTable();
            example.InsertRows();
            example.QueryTable();
            example.UpdateRow();
            example.QueryTable();
            example.DropTable();
        }
    }
}
