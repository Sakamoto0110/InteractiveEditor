using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.BindingInterface
{
    public interface IVarProvider
    {
        string[] VariablePool { get; set; }
    }
}
