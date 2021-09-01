using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace PortalAPI_Service.Repositories.DirectoryRepos
{
    public class DirRepo : IDirRepo
    {
        public async Task<IEnumerable<string>> GetTheSubFolder(string root_path)
        {
            return Directory.GetDirectories(root_path).ToList();

        }
    }
}
