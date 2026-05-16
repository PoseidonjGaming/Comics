using JDownloader.Namespace;

namespace ComicsJDownloaderApi.Namespace
{
    public class CaptchaForward(ComicsJDownloaderClient client) :
        BaseNamespace(client), ICaptchaForward
    {
        public override string Endpoint => "captchaforward";

        public Task<long> CreateJobRecaptchaV2(string siteKey, string siteToken, string siteDomain, string reason)
        {
            return PostRequestAsync<long>("createJobRecaptchaV2", new object[4] { siteKey, siteToken, siteDomain, reason });
        }

        public Task<string> GetResult(long captchaJobId)
        {
            return PostRequestAsync<string>("getResult", new object[1] { captchaJobId });
        }
    }
}