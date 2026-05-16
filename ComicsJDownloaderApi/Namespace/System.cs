using JDownloader.Model;
using JDownloader.Namespace;

namespace ComicsJDownloaderApi.Namespace
{
    public class System(ComicsJDownloaderClient client) :
        BaseNamespace(client), ISystem
    {
        public override string Endpoint => "system";

        public Task<bool> ExitJD()
        {
            return PostRequestAsync<bool>("exitJD");
        }

        public Task<List<StorageInformation>> GetStorageInfos(string path)
        {
            return PostRequestAsync<List<StorageInformation>>("getStorageInfos", new object[1] { path });
        }

        public Task<SystemInformation> GetSystemInfos()
        {
            return PostRequestAsync<SystemInformation>("getSystemInfos");
        }

        public Task<bool> HibernateOS()
        {
            return PostRequestAsync<bool>("hibernateOS");
        }

        public Task<bool> RestartJD()
        {
            return PostRequestAsync<bool>("restartJD");
        }

        public Task<bool> ShutdownOS(bool forceShutdown)
        {
            return PostRequestAsync<bool>("shutdownOS", new object[1] { forceShutdown });
        }

        public Task<bool> StandbyOS()
        {
            return PostRequestAsync<bool>("standbyOS");
        }
    }
}