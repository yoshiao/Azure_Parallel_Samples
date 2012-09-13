using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Configuration;
using System.Data;
using System.IO;

namespace SqlAzureExample
{
    class BlobShardingExample
    {
        String connectionString;
        String containerName = "images";

        public BlobShardingExample(String server, String database, String login, String password)
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

        public void CreateTable()
        {
            String commandText =
              @"CREATE TABLE Image (
                  Id int PRIMARY KEY NOT NULL,
                  Tag nvarchar(64) NOT NULL,
                  ImageName nvarchar(64) NOT NULL)";
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
            String commandText = "DROP TABLE Image";
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

        public String RetrieveImageName(String tag)
        {
            String commandText =
               @"SELECT TOP 1 ImageName
                 FROM Image
                 WHERE
                 Tag = @Tag";

            String imageName = String.Empty;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(commandText, connection))
                {
                    sqlCommand.Parameters.Add("@Tag", SqlDbType.NVarChar, 64).Value = tag;
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        Int32 blobNameColumn = reader.GetOrdinal("ImageName");
                        while (reader.Read())
                        {
                            imageName = reader[blobNameColumn] as String;
                        }
                    }
                }
            }
            return imageName;
        }

        public void SaveImageName( Int32 Id, String tag, String imageName)
        {
            String commandText =
              @"INSERT INTO Image
                (Id, Tag, ImageName)
                VALUES
                ( @Id, @Tag, @ImageName)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(commandText, connection))
                {
                    sqlCommand.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                    sqlCommand.Parameters.Add("@Tag", SqlDbType.NVarChar, 64).Value = tag;
                    sqlCommand.Parameters.Add("@ImageName", SqlDbType.NVarChar, 64).Value = imageName;
                    sqlCommand.ExecuteNonQuery();
                }
            }
            return;
        }

        public void UploadImage(String imageName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["DataConnectionString"]);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
            cloudBlobContainer.CreateIfNotExist();
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);

            String shouldBeAnImage = new String('z', 1000);
            UTF8Encoding utf8Encoding = new UTF8Encoding();
            using (MemoryStream memoryStream = new MemoryStream(utf8Encoding.GetBytes(shouldBeAnImage)))
            {
                cloudBlockBlob.UploadFromStream(memoryStream);
            }

            return;
        }

        public void Save(String tag, String imageName)
        {
            UploadImage(imageName);
            SaveImageName(1, tag, imageName);
        }

        public static void UseBlobShardingExample()
        {
            String server = "SERVER_NAME";
            String database = "DATABASE_NAME";
            String login = "LOGIN";
            String password = "PASSWORD";
            String tag = "Grand Canyon";
            String imageName = "SomeImage";

            BlobShardingExample example = new BlobShardingExample(server, database, login, password);
            example.CreateTable();
            example.Save(tag, imageName);
            String retrievedBlobName = example.RetrieveImageName(tag);
            example.DropTable();
        }
    }
}
