using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebFramework;

public class RouteTree : IRouteTree
{
    private readonly RouteTreeNode _rootNode;
    private readonly IIoCContainer _iocContainer;
    private readonly IControllerTypeProvider _controllerTypeProvider;
    private readonly IControllerMethodTypeProvider _controllerMethodTypeProvider;

    public RouteTree(
        IIoCContainer ioCContainer, 
        string prefix, 
        IControllerTypeProvider controllerTypeProvider, 
        IControllerMethodTypeProvider controllerMethodTypeProvider)
    {
        _iocContainer = ioCContainer;
        _rootNode = new RouteTreeNode(prefix, null);
        _controllerTypeProvider = controllerTypeProvider;
        _controllerMethodTypeProvider = controllerMethodTypeProvider;
    }

    public void BuildRouteTree<TController>() where TController : class
    {
        var controllerTypes = _controllerTypeProvider.GetControllerTypes<TController>();

        foreach (var controllerType in controllerTypes)
        {
            var controllerRoute = controllerType.GetCustomAttribute<RouteAttribute>()!.Path;

            var methods = _controllerMethodTypeProvider.GetMethods(controllerType);
            var lastNode = AddControllerRouteToRouteTree(controllerRoute);

            foreach (var method in methods)
            {
                var methodRoute = method.GetCustomAttribute<RouteAttribute>()!.Path;
                var parameters = new object[0];
                var func = () => InvokeMethod(method, controllerType, parameters);
                AddMethodRouteToRouteTree(lastNode, methodRoute, func);
            }
        }

    }

    public Func<object?> GetEndpoint(string path)
    {
        var regex = new Regex(@"[{][^{]+[}]");
        var unificatedPath= regex.Replace(path, "{*}");
        var pathSegments = unificatedPath.Split('/');

        var currentNode = _rootNode;
        if (currentNode.SegmentName != pathSegments[0])
        {
            throw new Exception("Method does not exists.");
        }

        for (int i = 1; i < pathSegments.Length; i++)
        {
            currentNode = currentNode.GetChild(pathSegments[i]);

            if (currentNode == null)
            {
                throw new Exception("Method does not exists.");
            }
        }

        if (currentNode.Function != null)
        {
            return currentNode.Function;
        }
        else
        {
            throw new Exception("Method does not exists.");
        }
    }

    private RouteTreeNode AddControllerRouteToRouteTree(string controllerRoute)
    {
        var controllerSegments = controllerRoute.Split("/");
        RouteTreeNode currentNode = new RouteTreeNode(controllerSegments[0]);
        var alreadyAddedChild = _rootNode.GetChild(controllerSegments[0]);
        if (alreadyAddedChild != null)
        {
            currentNode = alreadyAddedChild;
        }
        else
        {
            _rootNode.AddChild(currentNode);
        }

        for (int i = 1; i < controllerSegments.Length; i++)
        {
            alreadyAddedChild = currentNode.GetChild(controllerSegments[i]);
            if (alreadyAddedChild != null)
            {
                currentNode = alreadyAddedChild;
            }
            else
            {
                var node = new RouteTreeNode(controllerSegments[i]);
                currentNode.AddChild(node);
                currentNode = node;
            }
        }

        return currentNode;
    }

    private void AddMethodRouteToRouteTree(RouteTreeNode lastNode, string methodRoute, Func<object?> function)
    {
        var regex = new Regex(@"[{][^{]+[}]");
        var unificatedMethodRoute = regex.Replace(methodRoute, "{*}");

        var methodSegments = unificatedMethodRoute.Split("/");
        var currentNode = lastNode;

        for (int i = 0; i < methodSegments.Length; i++)
        {
            if (i < methodSegments.Length - 1)
            {
                var node = currentNode.GetChild(methodSegments[i]);
                if (node == null)
                {
                    node = new RouteTreeNode(methodSegments[i]);
                    currentNode.AddChild(node);
                }
                
                currentNode = node;
                //alreadyAddedChild = currentNode.GetChild(methodSegments[i]);
                //if (alreadyAddedChild != null)
                //{
                //    currentNode = alreadyAddedChild;
                //}
                //else
                //{
                //    var node = new RouteTreeNode(methodSegments[i]);
                //    currentNode.AddChild(node);
                //    currentNode = node;
                //}
            }
            else
            {
                if (currentNode.GetChild(methodSegments[i]) == null)
                {
                    var node = new RouteTreeNode(methodSegments[i], function);
                    currentNode.AddChild(node);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }

    private object? InvokeMethod(MethodInfo method, Type controllerType, object[] parameters)
    {
        var controller = _iocContainer.Resolve(controllerType);
        return method.Invoke(controller, parameters);
    }

    private class RouteTreeNode {
        private readonly Dictionary<string, RouteTreeNode> _childNodes = new Dictionary<string, RouteTreeNode>();

        public string SegmentName { get; private set; }
        public Func<object?>? Function { get; private set; }

        public RouteTreeNode(string segmentName, Func<object?>? function = null)
        {
            SegmentName = segmentName;
            Function = function;
        }

        public void AddChild(RouteTreeNode node)
        {
            if (node.SegmentName == "{*}")
            {
                if (_childNodes.Count == 0)
                {
                    _childNodes.Add(node.SegmentName, node);
                }
                //else if (!(_childNodes.Count == 1 && ContainsChild("{*}")))
                //{
                //    throw new InvalidOperationException();
                //}
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                _childNodes.Add(node.SegmentName, node);
            }
        }

        public RouteTreeNode? GetChild(string segmentName)
        {
            return _childNodes.GetValueOrDefault(segmentName);
        }

        public bool ContainsChild(string segmentName)
        {
            return _childNodes.ContainsKey(segmentName);
        }
    }
}

