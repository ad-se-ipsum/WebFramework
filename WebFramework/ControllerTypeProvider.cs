using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework;

public class ControllerTypeProvider : IControllerTypeProvider
{
    public IEnumerable<Type> GetControllerTypes<TController>() where TController : class
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => type.IsSubclassOf(typeof(TController))
                && !type.IsAbstract
                && type.GetCustomAttribute<RouteAttribute>() != null);
    }
}
