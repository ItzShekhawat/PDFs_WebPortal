using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PortalAPI_Service.Repositories.FoldersRepos
{
    public class FoldersRepo : IFoldersRepo 
    {
        private readonly IDbConnection _db;
        public FoldersRepo(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<IEnumerable<GenericFF_Model>> GetSubFolders(string Father_Key, string TableName)
        {
            var query = $@"SELECT * FROM {TableName} WHERE FK_Father = '{Father_Key}'";
            //SELECT * FROM client WHERE FK_Father = 'Commesse'
            Console.WriteLine(TableName);
            Console.WriteLine(Father_Key);
            var result = await _db.QueryAsync<GenericFF_Model>(query);
            return result.ToList();
        }

        #region "Param Passing" 
        /*
        var param = new { Table = TableName, FK = Father_Key };
        var parameters = new DynamicParameters(param);

        var query = @"SELECT * FROM @Table WHERE FK_Father = '@FK'";
        //SELECT * FROM client WHERE FK_Father = 'Commesse'
        Console.WriteLine(TableName);
        var result = await _db.QueryAsync<GenericFF_Model>(query, parameters);
        return result.ToList();
        */
        #endregion

        public async Task<IEnumerable<PDFModel>> GetPDFAsync(string Suborder)
        {
            var query = @"SELECT * FROM pdf WHERE FK_Father = @suborder";
            var result = await _db.QueryAsync<PDFModel>(query, new { suborder = Suborder});
            return result.ToList();
        }

        public async Task<IEnumerable<PDF_FileModel>> GetPDF_FileAsync(int PDF)
        {
            var query = @"SELECT * FROM pdf_file WHERE FK_Father = @pdf";
            var result = await _db.QueryAsync<PDF_FileModel>(query, new { pdf = PDF});
            return result.ToList();
        }

        /*
        public async Task<IEnumerable<ClientModel>> GetClientsAsync()
        {
            var query = "SELECT * FROM Client";
            var result = await _db.QueryAsync<ClientModel>(query);
            return result.ToList();
        }
        public async Task<IEnumerable<OrderModel>> GetOrdersAsync(string Client)
        {
            var query = @"SELECT * FROM Orders WHERE FK_client = @client";
            Console.WriteLine(Client);
            var result = await _db.QueryAsync<OrderModel>(query,new { client = Client});
            return result.ToList();
        }
        public async Task<IEnumerable<Sub_orderModel>> GetSubordersAsync(string Order)
        {
            var query = @"SELECT * FROM sub_order WHERE FK_order = @order";
            var result = await _db.QueryAsync<Sub_orderModel>(query, new { order= Order});
            return result.ToList();
        }
        */


        
    }
}
