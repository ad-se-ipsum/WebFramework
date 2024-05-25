using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework;

public class ControllerMethodTypeProvider : IControllerMethodTypeProvider
{
    public IEnumerable<MethodInfo> GetMethods(Type type)
    {
        return type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(type => type.GetCustomAttribute<RouteAttribute>() != null);
    }
}
