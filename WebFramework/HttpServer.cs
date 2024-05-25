using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework;

public class HttpServer : IDisposable
{
    private readonly HttpListener _httpListener;
    private readonly HttpServerOptions _serverOptions;
    private readonly List<HttpRequestListener> _requestListeners;

    public HttpServer(HttpListener httpListener, HttpServerOptions httpServerOptions, RouteTree routeTree)
    {
        _serverOptions = httpServerOptions;
        _httpListener = httpListener;
        _httpListener.Prefixes.Add(_serverOptions.HttpListenerUri);

        _requestListeners = new List<HttpRequestListener>(_serverOptions.RequestListenersCount);
        for (var i = 0;  i < _serverOptions.RequestListenersCount; i++)
        {
            _requestListeners.Add(new HttpRequestListener(_httpListener, routeTree));
        }
    }

    public async Task Start()
    {
        _httpListener.Start();

        var requestListenerRunnerTasks = new List<Task>(_requestListeners.Count);
        for (var i = 0; i < _requestListeners.Count; i++)
        {
            requestListenerRunnerTasks.Add(_requestListeners[i].Run());
        }

        await Task.WhenAll(requestListenerRunnerTasks);
    }

    public void Stop()
    { 
        _httpListener.Stop();
    }

    public void Close()
    {
        _httpListener.Close();
    }

    public void Dispose()
    {
    }
}

public class HttpRequestListener
{
    private readonly HttpListener _httpListener;
    private readonly RouteTree _routeTree;

    public HttpRequestListener(HttpListener httpListener, RouteTree routeTree)
    {
        _httpListener = httpListener;
        _routeTree = routeTree;
    }

    public async Task Run()
    {
        while (true)
        {
            var context = await _httpListener.GetContextAsync();
            using var response = context.Response;
            var unificatedPath = context.Request.RawUrl.Substring(1);
            var method = _routeTree.GetEndpoint(unificatedPath);
            if (method != null)
            {
                var result = method.Invoke();
            }
            response.StatusCode = (int)HttpStatusCode.OK;
        }
    }
}

public class HttpServerOptions
{
    public int RequestListenersCount { get; set; } = 1;
    public string HttpListenerUri { get; set; } = string.Empty;
}