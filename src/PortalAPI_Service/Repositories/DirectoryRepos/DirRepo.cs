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
                    if(TableName == "PDF" || TableName == "pdf")
                    {
                        if (path.Contains("PDF"))  // This way Only the PDF Dir will be storad
                        {
                            slash_path = path.Trim().Split(@"\");
                            F_name = slash_path.Last();
                            FK = slash_path[^2];


                            string sql;
                            if (!doUpdate)
                            {
                                sql = $"INSERT INTO {TableName} VALUES('{F_name}', '{path}', '{FK}')";
                                var result = await _db.ExecuteAsync(sql, new { table = TableName, name = F_name, location = path, fk = FK });

                            }
                            else
                            {
                                sql = $"SELECT Count(FF_Name) FROM {TableName} WHERE FF_Name = '{F_name}'";
                                var result = await _db.QueryAsync<int>(sql, new { table = TableName, name = F_name });

                                if (result.Contains(0))
                                {
                                    sql = $"INSERT INTO {TableName} VALUES('{F_name}', '{path}', '{FK}')";
                                    var UpdateResult = await _db.ExecuteAsync(sql, new { table = TableName, name = F_name, location = path, fk = FK });

                                }

                            }

                        }
                        
                    }
                    else
                    {
                        slash_path = path.Trim().Split(@"\");
                        F_name = slash_path.Last();
                        FK = slash_path[^2];


                        Console.WriteLine($"{slash_path}\n{F_name}|\n{FK}\n{path} ");

                        string sql;
                        if (!doUpdate)
                        {
                            sql = $"INSERT INTO {TableName} VALUES('{F_name}', '{path}', '{FK}')";
                            var result = await _db.ExecuteAsync(sql);

                            
                        }
                        else
                        {
                            sql = $"SELECT Count(FF_Name) FROM {TableName} WHERE FF_Name = '{F_name}'";
                            var result = await _db.QueryAsync<int>(sql, new { table = TableName, name = F_name });

                            if (result.Contains(0))
                            {
                                sql = $"INSERT INTO {TableName} VALUES('{F_name}', '{path}', '{FK}')";
                                _ = await _db.ExecuteAsync(sql);
                            }
                            
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


        // This function will Update or UpLoad File Data in the DB
        public async Task<bool> Upload_Update_File(List<string> FileList, bool doUpdate)
        {
            // "Z:\\SPAC\\Commesse\\ITT\\12ITT Macchina Pastiglie\\12001\\PDF\\Karni_Singh_Shekhawat_CV.pdf"

            string[] slash_path;
            string F_name;
            string FK;
            

            try
            {
                foreach(var path in FileList)
                {

                    slash_path = path.Trim().Split(@"\");
                    F_name = slash_path.Last();
                    Console.WriteLine(slash_path[^1]);
                    FK = path.Substring(0, path.LastIndexOf(@"\")).Trim();

                    string sql;
                    if (!doUpdate)
                    {
                        sql = $"SELECT Id FROM pdf WHERE Location_path = '{FK}'";
                        var result = await _db.QueryAsync<string>(sql);

                        sql = $"INSERT INTO pdf_file VALUES('{F_name}', '{path}', '{result.Single()}')";
                        _ = await _db.ExecuteAsync(sql);
                     
                       
                    }
                    else
                    {
                        sql = $"SELECT Count(FF_Name) FROM pdf_file WHERE FF_Name = '{F_name}'";
                        var result = await _db.QueryAsync<int>(sql);

                        if (result.Contains(0))
                        {
                            sql = $"SELECT Id FROM pdf WHERE Location_path = '{FK}'";
                            result = await _db.QueryAsync<int>(sql);

                            sql = $"INSERT INTO pdf_file VALUES('{F_name}', '{path}', '{result.Single()}')";
                            var UpdateResult = await _db.ExecuteAsync(sql);

                           
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
    }
}
