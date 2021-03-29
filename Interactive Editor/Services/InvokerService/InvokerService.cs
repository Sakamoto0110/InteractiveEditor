using Editor.Fields;
using Editor.Services;
using System;


namespace Editor.Services
{
    public class InvokerService : IInvokerService
    {
        public IOBServiceProvider Provider { get; set; }

        private FieldLocatorService FieldLocator => Provider.Request<FieldLocatorService>();

        
        public void InvokeForAllFields(Action<Fieldset> method)
        {
            foreach(Fieldset field in FieldLocator)
            {
                method?.Invoke(field);
            }
        }

      
    }
}
