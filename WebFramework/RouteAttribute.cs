using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RouteAttribute : Attribute
{
    public string Path { get; set; }
    public RouteAttribute(string path) => Path = path;
}