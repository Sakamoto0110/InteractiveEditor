using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.BindingInterface
{
    
    public interface ITwoWayBinderReciever
    {
        void EditFieldValueByVariableName(string target, object value);
    }

}
