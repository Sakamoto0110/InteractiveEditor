using Editor.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Services
{
    public interface IInvokerService : _IService
    {
        
        void InvokeForAllFields(Action<Fieldset> method);
        void InvokeForAllFieldsEx(Action<Fieldset> method);
        void InvokeForAllPages(Action<InteractiveEditor> method);

        void InvokeForAllPagesReverse(Action<InteractiveEditor> method);

    }
}
