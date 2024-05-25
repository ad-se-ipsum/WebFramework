using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework;

public interface IRouteTree
{
    void BuildRouteTree<TController>() where TController : class;
    Func<object?> GetEndpoint(string path);
}
