using PortalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalAPI_Service.Repositories.AccessRepos
{
    public interface ILoginRepo
    {
        public  Task<UsersModel> CheckUser(string username, string password);

        public List<UsersModel> AllUsers();
    }
}
