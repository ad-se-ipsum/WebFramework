using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework;

[Route("data")]
public class DataController : BaseController
{
    [Route("length")]
    public int GetLength()
    {
        var arr = new int[5];
        arr.GetHashCode();
        return 0;
    }

    [Route("length/{length}")]
    public int SaveLength(int length)
    {
        return length;
    }
}
