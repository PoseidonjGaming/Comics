using JDownloader.Model;
using JDownloader.Namespace;
using System.Text.Json;

namespace ComicsJDownloaderApi.Namespace
{
    public class AccountsV2(ComicsJDownloaderClient client) :
        BaseNamespace(client), IAccountsV2
    {
        public override string Endpoint => "accountsV2";
        public async Task<IEnumerable<Account>> ListAccounts(AccountQuery query)
        {
            return await PostRequestAsync<IEnumerable<Account>>("listAccounts", new object[1] { JsonSerializer.Serialize(query, base.JsonSerializerOptions) });
        }

        public async Task AddAccount(string premiumHosterUrl, string username, string password)
        {
            await PostRequestAsync("addAccount", new object[3] { premiumHosterUrl, username, password });
        }

        public async Task RemoveAccounts(long[] accountIds)
        {
            await PostRequestAsync("removeAccounts", new object[1] { accountIds });
        }

        public async Task RefreshAccounts(long[] accountIds, bool forceRefresh = false)
        {
            await PostRequestAsync("refreshAccounts", new object[2] { accountIds, forceRefresh });
        }

        public async Task<bool> SetUsernameAndPassword(long accountId, string username, string password)
        {
            return await PostRequestAsync<bool>("setUserNameAndPassword", new object[3] { accountId, username, password });
        }

        public async Task EnableAccounts(long[] accountIds)
        {
            await PostRequestAsync("enableAccounts", new object[1] { accountIds });
        }

        public async Task DisableAccounts(long[] accountIds)
        {
            await PostRequestAsync("disableAccounts", new object[1] { accountIds });
        }

        public async Task<string> GetPremiumHosterUrl(string hosterUrl)
        {
            return await PostRequestAsync<string>("getPremiumHosterUrl", new object[1] { hosterUrl });
        }

        public async Task<IEnumerable<string>> ListPremiumHoster()
        {
            return await PostRequestAsync<IEnumerable<string>>("listPremiumHoster");
        }

        public async Task<Dictionary<string, string>> ListPremiumHosterUrls()
        {
            return await PostRequestAsync<Dictionary<string, string>>("listPremiumHosterUrls");
        }

        public async Task<IEnumerable<ListBasicAuthResponse>> ListBasicAuth()
        {
            return await PostRequestAsync<IEnumerable<ListBasicAuthResponse>>("listBasicAuth");
        }

        public async Task<long> AddBasicAuth(HostType hostType, string hostmask, string username, string password)
        {
            return await PostRequestAsync<long>("addBasicAuth", new object[4]
            {
            hostType.ToString(),
            hostmask,
            username,
            password
            });
        }

        public async Task<bool> UpdateBasicAuth(ListBasicAuthResponse updatedEntry)
        {
            return await PostRequestAsync<bool>("updateBasicAuth", 
                new object[1] { JsonSerializer.Serialize(updatedEntry, base.JsonSerializerOptions) });
        }

        public async Task<bool> RemoveBasicAuths(long[] authenticationIds)
        {
            return await PostRequestAsync<bool>("removeBasicAuths", new object[1] { authenticationIds });
        }
    }
}
