using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortalAPI_Service.Repositories.DirectoryRepos
{
    public interface IDirRepo
    {
        Task<IEnumerable<string>> GetTheSubFolder(string root_path);
    }
}