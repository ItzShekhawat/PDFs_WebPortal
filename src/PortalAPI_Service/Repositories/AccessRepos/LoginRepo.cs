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
        readonly private IDbConnection _db;
        public LoginRepo(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<UsersModel> CheckUser(string username, string password)
        {
            string sql = $"SELECT * FROM users WHERE Full_name = '{username}' AND Password = '{password}' ";
            Console.WriteLine(sql);
            return  _db.Query<UsersModel>(sql).FirstOrDefault();
        }

        public List<UsersModel> AllUsers()
        {
            string sql = $"SELECT * FROM users";
            Console.WriteLine(sql);
            return _db.Query<UsersModel>(sql).ToList();
        }

        public void RemoveUser(int id)
        {
            string sql = $"DELETE users WHERE id = @ID";
            Console.WriteLine(sql);
            _ = _db.Execute(sql, new { ID = id });
        }

    }
}
