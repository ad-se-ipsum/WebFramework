using System.Net;

namespace WebFramework.Initializer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var iocContainer = new IoCContainer();
        iocContainer.Register<DataController>();
        iocContainer.Register<IControllerTypeProvider, ControllerTypeProvider>();
        iocContainer.Register<IControllerMethodTypeProvider, ControllerMethodTypeProvider>();

        var routeTree = new RouteTree(iocContainer, "cc", iocContainer.Resolve<IControllerTypeProvider>(), iocContainer.Resolve<IControllerMethodTypeProvider>());
        routeTree.BuildRouteTree<BaseController>();

        var httpListener = new HttpListener();
        var httpServerOptions = new HttpServerOptions()
        {
            RequestListenersCount = 2,
            HttpListenerUri = "http://127.0.0.1:9000/cc/"
        };

        using var server = new HttpServer(httpListener, httpServerOptions, routeTree);
        
        await server.Start();
    }
}