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


        // This function will Update or UpLoad Data in the DB
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
                            }
                            else
                            {
                                sql = @"UPDATE @table SET"
                                      + "FF_Name = '@name',"
                                      + "Location_path = '@location', "
                                      + "FK_Father = '@fk'";
                            }

                                var result = await _db.ExecuteAsync(sql, new { table = TableName, name = F_name, location = path, fk = FK });

                            return result != 0;
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
                        }
                        else
                        {
                            sql = @"UPDATE @table SET"
                                  + "FF_Name = '@name',"
                                  + "Location_path = '@location', "
                                  + "FK_Father = '@fk'";
                        }
                            Console.WriteLine(sql);
                        var result = await _db.ExecuteAsync(sql, new { table = TableName, name = F_name, location = path, fk = FK });

                        return result != 0;
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




    }
}
