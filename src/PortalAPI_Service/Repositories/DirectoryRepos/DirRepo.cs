using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using PortalModels;
using Dapper;
using System.Text.RegularExpressions;

namespace PortalAPI_Service.Repositories.DirectoryRepos
{
    public class DirRepo : IDirRepo
    {

        private readonly IDbConnection _db;
        public DirRepo(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }


        public async Task<IEnumerable<string>> GetTheSubFolder_File(string root_path, bool File_or_Folder )
        {
            // The bool will tell the function if he has to search of files or dir 
            return File_or_Folder ? Directory.GetDirectories(root_path).ToList() : Directory.GetFiles(root_path).ToList();

        }

        #region "Folder Handling"
        // This function will Update or UpLoad Dir Data in the DB
        public async Task<bool> Upload_Update_Folders(List<string> DirList, string TableName, bool doUpdate)
        {
            // Divide the path in right Columns ( Name, path, FK)

            string F_name;
            string FK;
            string[] slash_path;
       
            try
            {
                foreach (var path in DirList)
                {
                    slash_path = path.Trim().Split(@"\");
                    F_name = slash_path.Last();
                    FK = slash_path[^2];

                    Console.WriteLine(DateTime.Now);
                    if (TableName == "PDF" || TableName == "pdf")
                    {   /*
                        if (F_name != "PDF"|| F_name != "pdf")
                        {
                            Console.WriteLine("Element don't have a pdf in thair name");
                            continue;
                            
                        }
                        */

                        if(F_name.Contains("PDF") || F_name.Contains("pdf"))
                        {
                            if (!doUpdate)
                            {
                                try
                                {
                                    var query  = $"INSERT INTO {TableName} VALUES('{F_name}', '{path}', '{FK}')";
                                    var result = await _db.ExecuteAsync(query);
                                    continue;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error on Insert : " + ex.Message);
                                    continue;
                                }

                            }
                            else
                            {
                                var query = $"SELECT Count(FF_Name) FROM {TableName} WHERE FF_Name = '{F_name}'";
                                var result = await _db.QueryAsync<int>(query, new { table = TableName, name = F_name });

                                if (result.Single() == 0)
                                {
                                    Console.WriteLine("Does not exist");
                                    query = $"INSERT INTO {TableName} VALUES('{F_name}', '{path}', '{FK}')";
                                    _ = await _db.ExecuteAsync(query);
                                    continue;
                                }
                                else
                                {
                                    Console.WriteLine("Does Exist");
                                    query = $"UPDATE {TableName} SET FF_Name = '{F_name}', Location_path = '{path}', FK_Father = '{FK}' WHERE Location_path = '{path}'";
                                    _ = await _db.ExecuteAsync(query);
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Element don't have a pdf in thair name");

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


                    Console.WriteLine($"{slash_path}\n{F_name}|\n{FK}\n{path} ");

                    string sql;
                    if (!doUpdate)
                    {
                        try
                        {
                            sql = $"INSERT INTO {TableName} VALUES('{F_name}', '{path}', '{FK}')";
                            var result = await _db.ExecuteAsync(sql);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error on Insert : " + ex.Message);
                            continue;
                        }

                    }
                    else
                    {
                        sql = $"SELECT Count(FF_Name) FROM {TableName} WHERE FF_Name = '{F_name}'";
                        var result = await _db.QueryAsync<int>(sql, new { table = TableName, name = F_name });

                        if (result.Single() == 0)
                        {
                            Console.WriteLine("Does not exist");
                            sql = $"INSERT INTO {TableName} VALUES('{F_name}', '{path}', '{FK}')";
                            _ = await _db.ExecuteAsync(sql);
                        }
                        else
                        {
                            Console.WriteLine("Does Exist");
                            sql = $"UPDATE {TableName} SET FF_Name = '{F_name}', Location_path = '{path}', FK_Father = '{FK}' WHERE Location_path = '{path}'";
                            _ = await _db.ExecuteAsync(sql);

                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Some thing went wrong in UploadFolder try : {ex.Message}");
                return false;
            }

        }
        #endregion




        #region " File Handling"
        // This function will Update or UpLoad File Data in the DB
        public async Task<bool> Upload_Update_File(List<string> FileList, bool doUpdate)
        {
            // "Z:\\SPAC\\Commesse\\ITT\\12ITT Macchina Pastiglie\\12001\\PDF\\Karni_Singh_Shekhawat_CV.pdf"

            string[] slash_path;
            string F_name;
            string Path_PDF_Folder;
            string cleanPath;
            string sql;



            try
            {
                foreach(var path in FileList)
                {
                    Console.WriteLine(path);
                    cleanPath = path.Replace(@"\\", @"\");
                    Console.WriteLine(cleanPath);

                    slash_path = path.Trim().Split(@"\");
                    F_name = slash_path.Last();
                    Console.WriteLine(slash_path[^1]);
                    Path_PDF_Folder = path.Substring(0, path.LastIndexOf(@"\")).Trim(); //This will help us take the ID of the PDF Folder

                    if (!doUpdate)
                    {
                        sql = $"SELECT Id FROM pdf WHERE Location_path = '{Path_PDF_Folder}'";
                        Console.WriteLine("ID Getting sql = " + sql);
                        var result = await _db.QueryAsync<int>(sql);
                        var ID_Folder = result.Single();
                        Console.WriteLine("The Id is : " +ID_Folder);

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
                            sql = $"SELECT FK_Father FROM pdf_file WHERE FF_Name = '{F_name}'";
                            var ID_Father = await _db.QueryAsync<int>(sql);

                            sql = $"INSERT INTO pdf_file VALUES('{F_name}', '{path}', {ID_Father})";
                            _ = await _db.ExecuteAsync(sql);
                        }
                        else
                        {
                            Console.WriteLine("Can't update. This file does not exist");
                            return false;
                        }
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
    }
}
