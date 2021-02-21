using Editor.Fields;
using Editor.Services;
using System;


namespace Editor.Services
{
    public class InvokerService : IInvokerService
    {
        public IOBServiceProvider Provider { get; set; }

        private FieldLocatorService FieldLocator => Provider.Request<FieldLocatorService>();
        private PageLocatorService PageLocator => Provider.Request<PageLocatorService>();

        public void InvokeForAllFieldsEx(Action<Fieldset> method)
        {
            InvokeForAllPages((e) => e.Invoker.InvokeForAllFields(method) );
        }

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
        public void InvokeForAllPagesReverse(Action<InteractiveEditor> method)
        {
            for(int i = PageLocator.Count-1; i >= 0; i--)
            {
                var page = PageLocator.LocateIndex(i);
                method?.Invoke(page);
            }
        }
    }
}
