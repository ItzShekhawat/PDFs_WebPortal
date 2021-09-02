using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortalAPI_Service.Repositories.DirectoryRepos
{
    public interface IDirRepo
    {
        Task<IEnumerable<string>> GetTheSubFolder_File(string root_path, bool File_or_Folder);

        Task<bool> Upload_Update_Folders(List<string> DirList, string TableName, bool doUpdate);

        Task<bool> Upload_Update_File(List<string> FileList, bool doUpdate);
    }
}