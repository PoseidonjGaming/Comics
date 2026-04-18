using DownloadComics.models;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Windows;

namespace DownloadComics.services
{
    public class ListenerService
    {
        private CancellationTokenSource? _listenTokenSource;
        private HttpListener? _listener;
        public TaskCompletionSource<List<OfflineLink>> TaskCompletionSource;

        private static readonly Lazy<ListenerService> _instance = new(() => new ListenerService());

        public static ListenerService Instance => _instance.Value;

        private ListenerService()
        {
            _listenTokenSource = null;
            TaskCompletionSource = new TaskCompletionSource<List<OfflineLink>>();
        }

        public Task? StartAsync(Func<string, bool> finishedProvider, int port = 12345)
        {
            if (_listenTokenSource != null && !_listenTokenSource.IsCancellationRequested)
                return null; // already started

            _listenTokenSource = new CancellationTokenSource();

            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{port}/");
            _listener.Start();

            return Task.Run(async () =>
            {
                try
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

                                    writer.Write(finishedProvider.Invoke(response.Replace("data=", string.Empty)));
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
                }
                catch (HttpListenerException)
                {

                }
                catch (OperationCanceledException ex)
                {
                    Application.Current.Dispatcher.Invoke(() => MessageBox.Show(ex.Message));
                }
                finally
                {
                    try
                    {
                        if (_listener != null)
                        {
                            try
                            {
                                if (_listener.IsListening)
                                    _listener.Stop();
                            }
                            catch (ObjectDisposedException) { }
                            catch (HttpListenerException) { }
                            catch (InvalidOperationException) { }

                            try { _listener.Close(); } catch { }
                        }
                    }
                    finally
                    {
                        _listener = null;
                    }
                }
            }, _listenTokenSource.Token);
        }

        public void Cancel()
        {
            if (_listenTokenSource == null) return;
            _listenTokenSource.Cancel();
        }

        public void Dispose()
        {
            Cancel();
            try { _listener?.Close(); } catch { }
            _listener = null;
            _listenTokenSource?.Dispose();
            _listenTokenSource = null;
        }

        public async Task<List<OfflineLink>> WaitJob()
        {
            List<OfflineLink> links = await TaskCompletionSource.Task;
            TaskCompletionSource = new();
            return links;
        }
    }
}
