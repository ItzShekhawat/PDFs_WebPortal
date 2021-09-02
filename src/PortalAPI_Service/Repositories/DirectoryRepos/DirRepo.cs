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
                    if(TableName == "PDF")
                    {
                        if (path.Contains("PDF"))  // This way Only the PDF Dir will be storad
                        {
                            slash_path = path.Trim().Split(@"\");
                            F_name = slash_path.Last();
                            FK = slash_path[^2];


                            string sql;
                            if (!doUpdate)
                            {
                                sql = @"INSERT INTO @table VALUES('@name', '@location', '@fk')";
                                var result = await _db.ExecuteAsync(sql, new { table = TableName, name = F_name, location = path, fk = FK });

                                return result != 0;
                            }
                            else
                            {
                                sql = @"SELECT Count(FF_Name) FROM @table WHERE FF_Name == '@name'";
                                var result = await _db.QueryAsync<int>(sql, new { table = TableName, name = F_name });

                                if (result.Contains(0))
                                {
                                    sql = @"INSERT INTO @table VALUES('@name', '@location', '@fk')";
                                    var UpdateResult = await _db.ExecuteAsync(sql, new { table = TableName, name = F_name, location = path, fk = FK });

                                    return UpdateResult != 0;
                                }

                                return true;
                            }

                        }
                        
                    }
                    else
                    {
                        slash_path = path.Trim().Split(@"\");
                        F_name = slash_path.Last();
                        FK = slash_path[^2];

                        //Console.WriteLine($"{slash_path}\n{F_name}|\n{FK}\n{path} ");

                        string sql;
                        if (!doUpdate)
                        {
                            sql = @"INSERT INTO @table VALUES('@name', '@location', '@fk')";
                            var result = await _db.ExecuteAsync(sql, new { table = TableName, name = F_name, location = path, fk = FK });

                            return result != 0;
                        }
                        else
                        {
                            sql = @"SELECT Count(FF_Name) FROM @table WHERE FF_Name == '@name'";
                            var result = await _db.QueryAsync<int>(sql, new { table = TableName, name = F_name });

                            if (result.Contains(0))
                            {
                                sql = @"INSERT INTO @table VALUES('@name', '@location', '@fk')";
                                _ = await _db.ExecuteAsync(sql, new { table = TableName, name = F_name, location = path, fk = FK });
                            }

                            return true;
                        }
                    }
                    
                }

                return false;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Some thing went wrong in UploadFolder try : {ex}");
                return false;
            }

        }


        // This function will Update or UpLoad File Data in the DB
        public async Task<bool> Upload_Update_File(List<string> FileList, bool doUpdate)
        {
            // "Z:\\SPAC\\Commesse\\ITT\\12ITT Macchina Pastiglie\\12001\\PDF\\Karni_Singh_Shekhawat_CV.pdf"

            string[] slash_path;
            string F_name;
            int FK;

            try
            {
                foreach(var path in FileList)
                {

                    slash_path = path.Trim().Split(@"\");
                    F_name = slash_path.Last();

                    string sql;
                    if (!doUpdate)
                    {
                        sql = @"SELECT Id FROM PDF WHERE Location_path == '@loc'";
                        var result = await _db.QueryAsync<int>(sql, new {loc = slash_path[slash_path.Length-1]});


                        sql = @"INSERT INTO pdf_file VALUES('@name', '@location', '@fk')";
                        var InsertResult = await _db.ExecuteAsync(sql, new { name = F_name, location = path, fk = result });

                        return InsertResult != 0;
                    }
                    else
                    {
                        sql = @"SELECT Count(FF_Name) FROM pdf_file WHERE FF_Name == '@name'";
                        var result = await _db.QueryAsync<int>(sql, new { name = F_name });

                        if (result.Contains(0))
                        {
                            sql = @"SELECT Id FROM PDF WHERE Location_path == '@loc'";
                            result = await _db.QueryAsync<int>(sql, new { loc = slash_path[slash_path.Length - 1] });

                            sql = @"INSERT INTO pdf_file VALUES('@name', '@location', '@fk')";
                            var UpdateResult = await _db.ExecuteAsync(sql, new {  name = F_name, location = path, fk = result });

                            return UpdateResult != 0;
                        }

                        return true;
                    }

                }


            }
            catch (Exception ex)
            {

                Console.WriteLine($"Some thing went wrong in UploadFolder try : {ex}");
                return false;
            }

            return false;
            

        }
    }
}
