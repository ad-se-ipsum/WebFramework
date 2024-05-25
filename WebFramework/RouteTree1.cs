using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework;

//public class RouteTree1 : IRouteTree
//{
//    private readonly ConcurrentDictionary<string, Func<object?>> _routeTree = new ConcurrentDictionary<string, Func<object?>>();
//    private readonly IIoCContainer _iocContainer;

//    public RouteTree1(IIoCContainer ioCContainer)
//    {
//        _iocContainer = ioCContainer;
//    }

//    public void BuildRouteTree<TController>(string prefix) where TController : class
//    {
//        var controllerTypes = Assembly.GetExecutingAssembly().GetTypes()
//            .Where(type => type.IsSubclassOf(typeof(TController))
//                && !type.IsAbstract
//                && type.GetCustomAttribute<RouteAttribute>() != null);

//        foreach (var controllerType in controllerTypes)
//        {
//            var controllerRoute = controllerType.GetCustomAttribute<RouteAttribute>()!.Path;

//            var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
//                .Where(type => type.GetCustomAttribute<RouteAttribute>() != null);

//            foreach (var method in methods)
//            {
//                var methodRoute = method.GetCustomAttribute<RouteAttribute>()!.Path;
//                var parameters = new object[0];
//                _routeTree[$"{prefix}/{controllerRoute}/{methodRoute}"] = () => InvokeMethod(method, controllerType, parameters);
//            }
//        }

//    }

//    public Func<object?> GetEndpoint(string path)
//    {
//        if (_routeTree.TryGetValue(path, out var method))
//        {
//            return method;
//        }

//        throw new Exception("Method does not exists.");
//    } 
    
//    private object? InvokeMethod(MethodInfo method, Type controllerType, object[] parameters)
//    {
//        var controller = _iocContainer.Resolve(controllerType);
//        return method.Invoke(controller, parameters);
//    }
//}
