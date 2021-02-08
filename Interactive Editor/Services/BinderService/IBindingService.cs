using Editor.Events;
using Editor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Editor.InteractiveEditor;
using static Editor.Modifiers;

namespace Editor.Services
{
    public interface IBindingService : _IService 
    {


        event EventHandler<BinderEventArgs> BindStarted;
        event EventHandler<BinderEventArgs> BindFinished;


        void BindToObject(object Instance, bool multiBind = false, Func<object, object, object, object> bruteForce = null);
        void UnbindObject();
        bool CheckIfFieldExists(string varName);
        void BindToVariable(string key, string varName, CapFunction f = null, object capParms = null);

    }
}
