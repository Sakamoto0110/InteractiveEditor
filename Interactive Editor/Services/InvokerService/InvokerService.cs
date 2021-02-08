using Editor.Fields;
using Editor.Services.ServiceProvider;
using System;


namespace Editor.Services
{
    public class InvokerService : IInvokerService
    {
        public IOBServiceProvider Provider { get; set; }

        private FieldLocatorService FieldLocator => Provider.Request<FieldLocatorService>();
        private PageLocatorService PageLocator => Provider.Request<PageLocatorService>();

        public void InvokeForAllFields(Action<Fieldset> method)
        {
            foreach(Fieldset field in FieldLocator)
            {
                method?.Invoke(field);
            }
        }

        public void InvokeForAllPages(Action<InteractiveEditor> method)
        {
            foreach (InteractiveEditor page in PageLocator)
            {
                method?.Invoke(page);
            }
        }
    }
}
