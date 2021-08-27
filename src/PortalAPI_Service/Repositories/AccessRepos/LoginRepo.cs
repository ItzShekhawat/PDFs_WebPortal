using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PortalAPI_Service.Repositories.AccessRepos
{
    public class LoginRepo : ILoginRepo
    {
        private IDbConnection _db;
        public LoginRepo(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<UsersModel> CheckUser(string username, string password)
        {
            string sql = $"SELECT Id, Full_name, Password, Access_token,  Department FROM users WHERE Full_name = '{username}' AND Password = '{password}' ";
            Console.WriteLine(sql);
            return _db.Query<UsersModel>(sql).FirstOrDefault();
        }

        public List<UsersModel> AllUsers()
        {
            string sql = $"SELECT Id, Full_name, Password, Access_token,  Department FROM users ;";
            Console.WriteLine(sql);
            return _db.Query<UsersModel>(sql).ToList();
        }

    }
}
