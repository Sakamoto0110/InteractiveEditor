using Editor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Services
{
    public interface _IService
    {
        IOBServiceProvider Provider { get; set; }
    }
}
