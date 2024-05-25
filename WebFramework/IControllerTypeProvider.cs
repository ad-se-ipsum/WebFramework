using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework;

public interface IControllerTypeProvider
{
    IEnumerable<Type> GetControllerTypes<TController>() where TController : class;
}
