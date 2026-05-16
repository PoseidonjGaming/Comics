using JDownloader.Model;
using JDownloader.Namespace;

namespace ComicsJDownloaderApi.Namespace
{
    public class Captcha(ComicsJDownloaderClient client) :
        BaseNamespace(client), ICaptcha
    {
        public override string Endpoint => "captcha";

        public Task<string> Get(long captchaId)
        {
            return PostRequestAsync<string>("get", new object[1] { captchaId });
        }

        public Task<CaptchaJob> GetCaptchaJob(long captchaJobId)
        {
            return PostRequestAsync<CaptchaJob>("getCaptchaJob", new object[1] { captchaJobId });
        }

        public Task<IEnumerable<CaptchaJob>> List()
        {
            return PostRequestAsync<IEnumerable<CaptchaJob>>("list");
        }

        public Task<bool> Skip(long captchaJobId, CaptchaSkipRequest skipType)
        {
            return PostRequestAsync<bool>("skip", new object[2]
            {
            captchaJobId,
            skipType.ToString()
            });
        }

        public Task<bool> Solve(long captchaId, string captchaAnswer)
        {
            return PostRequestAsync<bool>("solve", new object[2] { captchaId, captchaAnswer });
        }
    }
}
