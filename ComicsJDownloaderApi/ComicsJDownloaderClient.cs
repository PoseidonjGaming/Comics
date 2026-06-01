using ComicsJDownloaderApi.Helpers;
using ComicsJDownloaderApi.Model;
using JDownloader;
using JDownloader.Model;
using JDownloader.Namespace;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;

namespace ComicsJDownloaderApi
{
    public class ComicsJDownloaderClient : IJDownloaderClient, IDisposable
    {
        private readonly JDownloaderClientOptions _jDownloaderClientOptions;
        private readonly JsonSerializerOptions _serializerOptions;
        public JsonSerializerOptions SerializerOptions => _serializerOptions;

        private readonly SessionInfo _sessionInfo;

        private readonly DeviceInfo _deviceInfo;

        private readonly HttpClient _httpClient;

        private readonly bool _ownsHttpClient;

        private bool _disposed;

        public bool IsConnected => !string.IsNullOrEmpty(_sessionInfo.SessionToken);

        public bool IsDeviceSelected => !string.IsNullOrEmpty(_deviceInfo.DeviceId);

        public IAccountsV2 AccountsV2 { get; set; }

        public ICaptcha Captcha { get; set; }

        public ICaptchaForward CaptchaForward { get; set; }

        public IConfig Config { get; set; }

        public IContentV2 ContentV2 { get; set; }

        public IDevice Device { get; set; }

        public IDialogs Dialogs { get; set; }

        public IDownloadController DownloadController { get; set; }

        public IDownloadEvents DownloadEvents { get; set; }

        public IDownloadsV2 DownloadsV2 { get; set; }

        public IEvents Events { get; set; }

        public IExtensions Extensions { get; set; }

        public IExtraction Extraction { get; set; }

        public IFlash Flash { get; set; }

        public IJD JD { get; set; }

        public ILinkCrawler LinkCrawler { get; set; }

        public ILinkGrabberV2 LinkGrabberV2 { get; set; }

        public ILog Log { get; set; }

        public IPlugins Plugins { get; set; }

        public IPolling Polling { get; set; }

        public IReconnect Reconnect { get; set; }

        public ISession Session { get; set; }

        public ISystem System { get; set; }

        public IToolbar Toolbar { get; set; }

        public IUI UI { get; set; }

        public IUpdate Update { get; set; }

        public ComicsJDownloaderClient(bool ownsHttpClient)
        {
            _jDownloaderClientOptions = new JDownloaderClientOptions()
            {
                AppKey = "Download Comics"
            };
            _sessionInfo = new();
            _deviceInfo = new();
            _ownsHttpClient = ownsHttpClient;
            _httpClient = JDownloaderUtility.CreateHttpClient();
            _serializerOptions = JDownloaderUtility.CreateJsonSerializerOptions();

            AccountsV2 = new Namespace.AccountsV2(this);
            Captcha = new Namespace.Captcha(this);
            CaptchaForward = new Namespace.CaptchaForward(this);
            Config = new Namespace.Config(this);
            ContentV2 = new Namespace.ContentV2(this);
            Device = new Namespace.Device(this);
            Dialogs = new Namespace.Dialogs(this);
            DownloadController = new Namespace.DownloadController(this);
            DownloadEvents = new Namespace.DownloadEvents(this);
            DownloadsV2 = new Namespace.DownloadsV2(this);
            Events = new Namespace.Events(this);
            Extensions = new Namespace.Extensions(this);
            Extraction = new Namespace.Extraction(this);
            Flash = new Namespace.Flash(this);
            JD = new Namespace.JD(this);
            LinkCrawler = new Namespace.LinkCrawler(this);
            LinkGrabberV2 = new Namespace.LinkGrabberV2(this);
            Log = new Namespace.Log(this);
            Plugins = new Namespace.Plugins(this);
            Polling = new Namespace.Polling(this);
            Reconnect = new Namespace.Reconnect(this);
            Session = new Namespace.Session(this);
            System = new Namespace.System(this);
            Toolbar = new Namespace.Toolbar(this);
            UI = new Namespace.UI(this);
            Update = new Namespace.Update(this);
        }

        public void ClearDirectConnectionInfo()
        {
            _deviceInfo.IP = string.Empty;
            _deviceInfo.Port = null;
            _deviceInfo.AutoFallback = false;
        }

        public async Task Connect(string email, string password)
        {
            if (!IsConnected)
            {
                ArgumentNullException.ThrowIfNull(email);

                ArgumentNullException.ThrowIfNull(password);

                byte[] loginSecret = _jDownloaderClientOptions.GetLoginSecret(email, password);
                _sessionInfo.DeviceSecret = _jDownloaderClientOptions.GetDeviceSecret(email, password);
                string action = $"/my/connect?email={HttpUtility.UrlEncode(email)}&appkey={HttpUtility.UrlEncode(_jDownloaderClientOptions.AppKey)}";
                ConnectResponse connectResponse = await GetRequestAsync<ConnectResponse>(new GetRequestArg(action, loginSecret));
                _sessionInfo.SessionToken = connectResponse.SessionToken;
                _sessionInfo.RegainToken = connectResponse.RegainToken;
                _sessionInfo.ServerEncryptionToken = (IsConnected ? JDownloaderCrypto.GetEncryptionToken(loginSecret, connectResponse.SessionToken) : null);
                _sessionInfo.DeviceEncryptionToken = (IsConnected ? JDownloaderCrypto.GetEncryptionToken(_sessionInfo.DeviceSecret, connectResponse.SessionToken) : null);
            }
        }

        public async Task Disconnect()
        {
            if (IsConnected && _sessionInfo.ServerEncryptionToken != null)
            {
                string action = $"/my/disconnect?sessiontoken={HttpUtility.UrlEncode(_sessionInfo.SessionToken)}";
                await GetRequestAsync<DisconnectResponse>(new GetRequestArg(action,
                    _sessionInfo.ServerEncryptionToken));
                _sessionInfo.Clear();
            }
        }

        public async Task<DeviceList> ListDevices()
        {
            if (!IsConnected || _sessionInfo.ServerEncryptionToken == null)
            {
                throw new NotConnectedException();
            }

            string action = $"/my/listdevices?sessiontoken={HttpUtility.UrlEncode(_sessionInfo.SessionToken)}";
            return await GetRequestAsync<DeviceList>(new GetRequestArg(action, _sessionInfo.ServerEncryptionToken));
        }

        public async Task ReConnect()
        {
            if (!IsConnected || _sessionInfo.ServerEncryptionToken == null)
            {
                throw new NotConnectedException();
            }

            string action = $"/my/reconnect?sessiontoken={HttpUtility.UrlEncode(_sessionInfo.SessionToken)}&regaintoken={HttpUtility.UrlEncode(_sessionInfo.RegainToken)}";
            ConnectResponse connectResponse = await GetRequestAsync<ConnectResponse>(new GetRequestArg(action, _sessionInfo.ServerEncryptionToken));
            _sessionInfo.SessionToken = connectResponse.SessionToken;
            _sessionInfo.RegainToken = connectResponse.RegainToken;
            _sessionInfo.ServerEncryptionToken = (IsConnected ? JDownloaderCrypto.GetEncryptionToken(_sessionInfo.ServerEncryptionToken, connectResponse.SessionToken) : null);
            _sessionInfo.DeviceEncryptionToken = (IsConnected ? JDownloaderCrypto.GetEncryptionToken(_sessionInfo.DeviceSecret, connectResponse.SessionToken) : null);
        }

        public void SetDirectConnectionInfo(DirectConnectionInfo directConnectionInfo, bool autoFallback = true)
        {
            _deviceInfo.IP = directConnectionInfo.IP;
            _deviceInfo.Port = directConnectionInfo.Port;
            _deviceInfo.AutoFallback = autoFallback; ;
        }

        public void SetWorkingDevice(DeviceData device)
        {
            _deviceInfo.DeviceId = device.Id;
            ClearDirectConnectionInfo();
        }

        public void SetWorkingDevice(string deviceId)
        {
            _deviceInfo.DeviceId = deviceId;
            ClearDirectConnectionInfo();
        }

        private async Task<T> GetRequestAsync<T>(GetRequestArg arg) where T : BaseApiModel
        {
            long requestId = JDownloaderUtility.GetUniqueRequestId();
            string action = $"{arg.Action}&rid={requestId}";

            string signature = JDownloaderCrypto.GetSignature(action, arg.Key);
            action += $"&signature={signature}";

            try
            {
                string serverURL = JDownloaderUtility.GetServerUrl(action);
                using HttpResponseMessage httpResponse = await _httpClient.GetAsync(serverURL);

                string response = await httpResponse.Content.ReadAsStringAsync();
                if (httpResponse != null && httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    string decryptValue = await JDownloaderCrypto.Decrypt(response, arg.Key);
                    T? val = JsonSerializer.Deserialize<T>(decryptValue, _serializerOptions);
                    if (val != null && val.RequestId == requestId)
                    {
                        return val;
                    }

                    throw new InvalidRequestIdException();
                }

                throw new MyJDownloaderException(JsonSerializer.Deserialize<GenericApiError>(response)?.ToString());

            }
            catch (MyJDownloaderException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new MyJDownloaderException(ex.Message, ex);
            }


        }
        private async Task<string> GetDecryptedResponseBody(HttpResponseMessage response)
        {
            string result = await response.Content.ReadAsStringAsync();
            try
            {
                if (_sessionInfo.DeviceEncryptionToken != null)
                    return await JDownloaderCrypto.Decrypt(result, _sessionInfo.DeviceEncryptionToken);
                return result;
            }
            catch (FormatException)
            {
                return result;
            }
        }

        public async Task<T> PostRequestAsync<T>(PostRequestArg arg)
        {
            ValidateConfiguration();
            try
            {
                string baseUrl = (_deviceInfo.HasDirectConnectionInfo ? _deviceInfo.DirectConnectionUrl : "https://api.jdownloader.org");
                return await PostRequestInternalAsync<T>(baseUrl, arg);
            }
            catch (MyJDownloaderException) when (_deviceInfo.HasDirectConnectionInfo)
            {
                if (_deviceInfo.AutoFallback)
                {
                    return await PostRequestInternalAsync<T>("https://api.jdownloader.org", arg);
                }

                throw new DirectConnectionException();
            }
        }

        private void ValidateConfiguration()
        {
            if (!IsConnected)
            {
                throw new NotConnectedException();
            }

            if (!IsDeviceSelected)
            {
                throw new DeviceNotSelectedException();
            }
        }

        private async Task<T> PostRequestInternalAsync<T>(string baseUrl, PostRequestArg arg)
        {
            if (_sessionInfo.DeviceEncryptionToken != null)
            {
                long requestId = JDownloaderUtility.GetUniqueRequestId();

                string jsonArg = JsonSerializer.Serialize(new GenericApiRequest()
                {
                    Params = arg.Param,
                    RequestId = requestId,
                    Url = arg.Action
                }, _serializerOptions);
                string body = await JDownloaderCrypto.Encrypt(jsonArg, _sessionInfo.DeviceEncryptionToken);
                try
                {
                    using StringContent requestContent = new(body, Encoding.UTF8, "application/aesjson-jd");
                    string formattedURL = GetFormattedPostUrl(baseUrl, arg.Action);
                    using HttpResponseMessage httpResponse = await _httpClient.PostAsync(formattedURL, requestContent);
                    string response = await GetDecryptedResponseBody(httpResponse);
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        response = FixPropertyName.Fix<T>(response);
                        if (arg.DoubleJsonDecode)
                            response = JDownloaderUtility.DoubleJsonDecode(response);

                        if (arg.SkipDeserialization)
                            return (T)Convert.ChangeType(response, typeof(T));

                        GenericApiResponse<T> genericApiResponse = JsonSerializer
                            .Deserialize<GenericApiResponse<T>>(response, _serializerOptions) ?? new();

                        if (!genericApiResponse.IsValidRequest(requestId))
                            throw new InvalidRequestIdException();

                        return genericApiResponse.Data;
                    }

                    throw new MyJDownloaderException(JsonSerializer.Deserialize<GenericApiError>(response)?.ToString());
                }
                catch (MyJDownloaderException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new MyJDownloaderException(ex.Message, ex);
                }
            }

            throw new InvalidDataException();
        }
        private string GetFormattedPostUrl(string baseUrl, string action)
        {
            return baseUrl + "/t_" + HttpUtility.UrlEncode(_sessionInfo.SessionToken) + "_" + HttpUtility.UrlEncode(_deviceInfo.DeviceId) + action;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && _ownsHttpClient)
                {
                    _httpClient.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
