using ComicsLib.Models;
using Newtonsoft.Json;
using System.Net;

namespace ComicsInfraLib.Services
{
    public class ListenerService
    {
        private CancellationTokenSource? _listenTokenSource;
        private HttpListener? _listener;
        public TaskCompletionSource<List<OfflineLink>> TaskCompletionSource;
        private readonly AppState State = AppStateStore.Instance;
        private readonly ReaderWriterLockSlim _countLock = new();
        private int _count;
        public int Count
        {
            get
            {
               _countLock.EnterReadLock();
                try
                {
                    return _count;
                }
                finally
                {
                    _countLock.ExitReadLock();
                }
            }
            set
            {
                _countLock.EnterWriteLock();
                try
                {
                    _count = value;
                }
                finally
                {
                    _countLock.ExitWriteLock();
                }
            }
        }

        private static readonly Lazy<ListenerService> _instance = new(() => new ListenerService());

        public static ListenerService Instance => _instance.Value;

        public ListenerService()
        {
            _listenTokenSource = null;
            TaskCompletionSource = new TaskCompletionSource<List<OfflineLink>>();
        }

        public void StartAsync(int port = 12345)
        {
            if (_listenTokenSource != null && !_listenTokenSource.IsCancellationRequested)
                return; // already started

            _listenTokenSource = new CancellationTokenSource();

            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{port}/");
            _listener.Start();

            Task.Run(async () =>
            {
                while (!_listenTokenSource.Token.IsCancellationRequested)
                {
                    HttpListenerContext context = await _listener.GetContextAsync();

                    switch (context.Request.Url?.AbsolutePath)
                    {
                        case "/finished":
                            {
                                using StreamWriter writer = new(context.Response.OutputStream);
                                using StreamReader reader = new(context.Request.InputStream);

                                string response = reader.ReadToEnd();
                                if (int.TryParse(response.Replace("data=", string.Empty), out int requestCount))
                                {
                                    Count += requestCount;
                                }
                                writer.Write(Count == State.Comics.Count);
                                writer.Flush();
                                context.Response.Close();
                                break;
                            }
                        case "/offline":
                            {
                                using StreamReader reader = new(context.Request.InputStream);
                                string response = reader.ReadToEnd();

                                TaskCompletionSource.TrySetResult(JsonConvert.DeserializeObject<List<OfflineLink>>(response.Replace("data=", string.Empty)) ?? []);

                                using StreamWriter writer = new(context.Response.OutputStream);
                                writer.Write("OK");
                                writer.Flush();
                                context.Response.Close();
                            }

                            break;
                        default:
                            context.Response.StatusCode = 404;
                            context.Response.Close();
                            break;
                    }
                }
            }, _listenTokenSource.Token);
        }

        public void Dispose()
        {

            _listener?.Stop();
            _listenTokenSource?.Cancel();

            _listener?.Close();
            _listener = null;
        }

        public async Task<List<OfflineLink>> WaitJob()
        {
            List<OfflineLink> links = await TaskCompletionSource.Task;
            TaskCompletionSource = new();
            return links;
        }
    }
}
