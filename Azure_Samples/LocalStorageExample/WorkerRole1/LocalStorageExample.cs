using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure.ServiceRuntime;
using System.IO;
using System.Diagnostics;

// Need to configure Local Storage resource in ServiceDefinition.csdef 
//

namespace WorkerRole1
{
    class LocalStorageExample
    {
        static readonly String storageName = "WorkerStorage";

        String fileName;
        LocalResource localResource = RoleEnvironment.GetLocalResource(storageName);

        public LocalStorageExample(String fileName) { this.fileName = fileName; }

        public void WriteToLocalStorage()
        {
            String path = Path.Combine(localResource.RootPath, fileName);

            if (File.Exists(path))
            {
                Trace.TraceInformation("File exists");
            }

            FileStream writeFileStream = File.Create(path);
            using (StreamWriter streamWriter = new StreamWriter(writeFileStream))
            {
                streamWriter.Write("File stored in local storage");
            }
        }

        public void ReadFromLocalStorage()
        {
            String fileContent = string.Empty;
            String path = Path.Combine(localResource.RootPath, fileName);
            FileStream readFileStream = File.Open(path, FileMode.Open);
            try
            {
                using (StreamReader streamReader = new StreamReader(readFileStream))
                {
                    fileContent = streamReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Trace.TraceInformation(e.Message);
            }
            Trace.TraceInformation(fileContent);
        }

        public static void UseLocalStorageExample()
        {
            String fileName = "WorkerRoleStorage.txt";

            LocalStorageExample example = new LocalStorageExample(fileName);
            example.WriteToLocalStorage();
            example.ReadFromLocalStorage();
        }
    }
}
