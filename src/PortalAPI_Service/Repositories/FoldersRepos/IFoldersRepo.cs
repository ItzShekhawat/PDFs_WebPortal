using PortalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalAPI_Service.Repositories.FoldersRepos
{
    public interface IFoldersRepo
    {
        // Get Info from DB
        public Task<IEnumerable<ClientModel>> GetClientsAsync();
        public Task<IEnumerable<OrderModel>> GetOrdersAsync(string Client);
        public Task<IEnumerable<Sub_orderModel>> GetSubordersAsync(string Order);
        public Task<IEnumerable<PDFModel>> GetPDFAsync(string Suborder);
        public Task<IEnumerable<PDF_FileModel>> GetPDF_FileAsync(string PDF);


    }
}
