using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace PortalAPI_Service.Repositories.AccessRepos
{
    public class LoginRepo : ILoginRepo
    {
        readonly private IDbConnection _db;
        byte[] pass;

        public LoginRepo(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<UsersModel> CheckUser(string username, string password)
        {
            password = CalculateSHA256(password);
            string sql = $"SELECT * FROM users WHERE Full_name = '" + username.Replace("'", "''") + "' AND Password = '" + password + "' ";
            Console.WriteLine(sql);
            return await _db.QueryFirstAsync<UsersModel>(sql);
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

        private string CalculateSHA256(string str)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] hashValue;
            UTF8Encoding objUtf8 = new UTF8Encoding();
            hashValue = sha256.ComputeHash(objUtf8.GetBytes(str));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashValue.Length; i++)
            {
                builder.Append(hashValue[i].ToString("x2"));
            }
            return builder.ToString();

        }
    }
}
