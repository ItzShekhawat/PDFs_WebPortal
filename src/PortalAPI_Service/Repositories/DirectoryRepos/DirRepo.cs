using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;
using System.Security;

namespace PortalAPI_Service.Repositories.DirectoryRepos
{
    public class DirRepo : IDirRepo
    {

        private readonly IDbConnection _db;

        const int LOGON32_PROVIDER_DEFAULT = 0;
        //This parameter causes LogonUser to create a primary token.   
        const int LOGON32_LOGON_INTERACTIVE = 2;
        private readonly string username;
        private readonly string domain;
        private readonly string password;

        public DirRepo(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            username = configuration.GetConnectionString("Username");
            domain = configuration.GetConnectionString("Domain");
            password = configuration.GetConnectionString("Password");
        }

        // Get The Impersonated User Token 
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
        int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);


        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public async Task<IEnumerable<string>> GetTheSubFolder_File(string root_path, bool File_or_Folder )
        {
            // The bool will tell the function if he has to search of files or dir 
            Console.WriteLine(root_path);
            List<string> ResultList = new();
            try
            {
            

                SafeAccessTokenHandle safeAccessTokenHandle;
                bool returnValue = LogonUser(username, domain, password,
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                    out safeAccessTokenHandle);

                // Check if the user Impersonated Exist 
                if (false == returnValue)
                {
                    int ret = Marshal.GetLastWin32Error();
                    Console.WriteLine("LogonUser failed with error code : {0}", ret);
                    throw new System.ComponentModel.Win32Exception(ret);
                }

                using (safeAccessTokenHandle)
                {
                    //Console.WriteLine("Did LogonUser Succeed? " + (returnValue ? "Yes" : "No"));
                    //Console.WriteLine("Value of Windows NT token: " + safeAccessTokenHandle);
                    #pragma warning disable CA1416 // Validate platform compatibility

                    // Check the identity.
                    //Console.WriteLine("Before impersonation: " + WindowsIdentity.GetCurrent().Name);

                    // Use the token handle returned by LogonUser.
                    WindowsIdentity.RunImpersonated(safeAccessTokenHandle, () => {
                        var impersonatedUser = WindowsIdentity.GetCurrent().Name;

                        //IF true it get's me the list of dir else it get's me the files list  
                        ResultList =  File_or_Folder ? Directory.GetDirectories(root_path).ToList() : Directory.GetFiles(root_path).ToList();
                    });
                
                    // Check the identity.
                    //Console.WriteLine("After closing the context: " + WindowsIdentity.GetCurrent().Name);
                    #pragma warning disable CA1416 // Validate platform compatibility
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore in lettura File/Folder : " + ex.Message) ;
            }
            return ResultList;


        }


        #region "This function will Update or UpLoad Dir Data in the DB"
        public async Task<bool> Upload_Update_Folders(List<string> DirList, string TableName, bool doUpdate)
        {
            // Divide the path in right Columns ( Name, path, FK)
            string[] slash_path;
            string F_name;
            string FK;

            
            try
            {
                foreach (var path in DirList)
                {
                    if (path.Contains("20053"))
                    {
                        Console.WriteLine("Find it");
                    }

                    slash_path = path.Trim().Split(@"\");
                    F_name = slash_path.Last();
                    FK = slash_path[^2];
                    Console.WriteLine(DateTime.Now);

                    if(TableName == "suborder" || TableName == "SUBORDER")
                    {
                        int count = path.Count(c => Char.IsDigit(c));
                        if (count <= 4 )
                        {
                            Console.WriteLine("This Folder do not match the requirements ");
                            continue;
                        }
                    }
                    if (TableName == "client" || TableName == "Client")
                    {
                        int count = path.Count(c => Char.IsDigit(c));
                        if (count >= 2)
                        {
                            Console.WriteLine("Element has more the 2 numbers in the name");
                            continue;
                        }

                    }
                    if(TableName == "pdf")
                    {
                        if (!F_name.Contains("pdf", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }

                    await UpdateHandlerAsync(F_name, path, FK, doUpdate, TableName, _db);
                }
 
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Some thing went wrong in UploadFolder  : {ex.Message}");
                return false;
            }

            return true;
            

        }
        #endregion




        #region "This function will Update or UpLoad File Data in the DB"
        public async Task<bool> Upload_Update_File(List<string> FileList, bool doUpdate)
        {
            // "Z:\\SPAC\\Commesse\\ITT\\12ITT Macchina Pastiglie\\12001\\PDF\\Karni_Singh_Shekhawat_CV.pdf"

            string[] slash_path;
            string F_Name;
            string FK_Father;
            
            string sql;
            


            try
            {

                foreach(var path in FileList)
                {
                    Console.WriteLine(path);


                    slash_path = path.Trim().Split(@"\");
                    F_Name = slash_path.Last().Replace("'", "''");

                
                    if (!F_Name.Contains(".pdf"))
                    { 
                        continue;
                    }
                    else
                    {
                        FK_Father = slash_path[^3]; //This will help us take the ID of the PDF Folder


                        if (doUpdate)
                        {
                            try
                            {
                                var sql_query = $"SELECT COUNT(*) FROM pdf_file WHERE FF_Name = '{F_Name}'";
                                var UpdateRequestCheck = await _db.QueryAsync<int>(sql_query);

                                if(UpdateRequestCheck.Single() > 0)
                                {
                                    Console.WriteLine("The File Exist! " + F_Name + "in Table PDF_File");
                                }
                                else
                                {
                                    
                                    Console.WriteLine("The File does not Exist! " + F_Name + "in Table PDF_File");

                                    sql_query = $@"SELECT Id FROM pdf WHERE FK_Father = '{FK_Father}'";
                                    var id_Father = await _db.QueryAsync<int>(sql_query);

                                    // After getting the ID we use it to Insert in the PDF_File Table
                                    sql = $@"INSERT INTO pdf_file VALUES('{F_Name}', '{path.Replace("'", "''")}', {id_Father.Single()})";
                                    Console.WriteLine("Inserting sql = " + sql);
                                    _ = await _db.ExecuteAsync(sql);

                                }

                            }
                            catch (Exception ex)
                            {

                                Console.WriteLine("Error in File Update {"+F_Name+"} : " + ex.Message);
                                continue;
                            }
                        }


                        #region"old Method" 
                        /* 
                        if (!doUpdate)
                        {
                            sql = $"SELECT Id FROM pdf WHERE Location_path = '{Path_PDF_Folder}'";
                            Console.WriteLine("ID Getting sql = " + sql);
                            var result = await _db.QueryAsync<int>(sql);
                            var ID_Folder = result.Single();
                            Console.WriteLine("The Id is : " + ID_Folder);

                            // After getting the ID we use it to Insert in the PDF_File Table
                            sql = $@"INSERT INTO pdf_file VALUES('{F_name}', '{path}', {ID_Folder})";
                            Console.WriteLine("Inserting sql = " + sql);
                            _ = await _db.ExecuteAsync(sql);

                        }
                        else
                        {   // Check if it really exist, if we want to update it
                            sql = $"SELECT Count(FF_Name) FROM pdf_file WHERE FF_Name = '{F_name}'";
                            Console.WriteLine("Count result  sql = " + sql);
                            var resultCount = await _db.QueryAsync<int>(sql);
                            var File_count = resultCount.Single();
                            Console.WriteLine($"There is {File_count} files");

                            if (File_count > 0)
                            {
                                //sql = $"SELECT FK_Father FROM pdf_file WHERE FF_Name = '{F_name}'";
                                //var ID_Father = await _db.QueryAsync<int>(sql);

                                sql = $"UPDATE pdf_file SET FF_Name = '{F_name}', Location_path = '{path}' WHERE FK_Father = {File_count}";
                                _ = await _db.ExecuteAsync(sql);
                            }
                            else
                            {
                                // file non esistente 
                                Console.WriteLine(" This file does not exist");

                                sql = $"SELECT Id FROM pdf WHERE Location_path = '{Path_PDF_Folder}'";
                                Console.WriteLine("ID Getting sql = " + sql);
                                var result = await _db.QueryAsync<int>(sql);
                                var ID_Folder = result.Single();
                                Console.WriteLine("The Id is : " + ID_Folder);

                                // After getting the ID we use it to Insert in the PDF_File Table
                                sql = $@"INSERT INTO pdf_file VALUES('{F_name}', '{path}', {ID_Folder})";
                                Console.WriteLine("Inserting sql = " + sql);
                                _ = await _db.ExecuteAsync(sql);
                            }
                        } */
                        #endregion
                    }


                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Some thing went wrong in UploadFile : {ex.Message}");
                return false;
            }
        }

        #endregion




        private async static Task UpdateHandlerAsync(string F_Name, string Location, string FKey, bool doUpdate, string TableName, IDbConnection _db)
        {
            F_Name = F_Name.Replace("'", "''");

            Location = Location.Replace("'", "''");
            string sql_query;


            if (doUpdate)
            {
                
                try
                {
                    // This will check of existing Folder
                    

                    if(TableName == "pdf") {
                        sql_query = $"SELECT Count(*) FROM {TableName} WHERE FK_Father = '{FKey.Replace("'", "''")}'"; 
                    }
                    else
                    {
                        sql_query = $"SELECT Count(*) FROM {TableName} WHERE FF_Name = '{F_Name}'";
                    }


                    var count_result = await _db.QueryAsync<int>(sql_query);

                    if (count_result.Single() > 0)
                    {
                        Console.WriteLine("The folder Exist! " + FKey + " in Table : " + TableName);

                    }
                    else
                    {
                        Console.WriteLine("The folder does not Exist! " + F_Name + "in Table : " + TableName);


                        sql_query = $"INSERT INTO {TableName} VALUES('{F_Name}', '{Location}', '{FKey.Replace("'", "''")}')";

                        

                        _ = await _db.ExecuteAsync(sql_query);

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in Update : " + ex.Message);

                }

            }
            else
            {
                try
                {
                    Console.WriteLine("Requested an Insert ! " + F_Name + "in Table : " + TableName);

                    sql_query = $"INSERT INTO {TableName} VALUES('{F_Name}', '{Location}', '{FKey.Replace("'", "''")}')";
                    _ = await _db.ExecuteAsync(sql_query);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error on Insert(Not Update) " + ex.Message);
                }
            }
        }
    }
}
