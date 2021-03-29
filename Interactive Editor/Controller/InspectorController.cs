using Editor.Services;
using Editor.Services.BinderService.Mapping;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Editor.Services.BindingService;

namespace Editor.Controller
{
    public class InspectorController
    {


        private Inspector MyInspector;


        public IFieldLocatorService LocateField => MyInspector.ServiceProvider.Request<FieldLocatorService>();



        public IManipulatorService Modify => MyInspector.ServiceProvider.Request<ManipulatorService>();


        public IBindingService Binder => MyInspector.ServiceProvider.Request<BindingService>();


        public IInvokerService Invoker => MyInspector.ServiceProvider.Request<InvokerService>();


        private InspectorController()
        {

            
        }

        public static InspectorController BuildInspector<T>(string name, Point location, Size size, BindingConfigurator activeConfigurator = null)
        {
            var inspector = new InspectorController()
            {
                MyInspector = Inspector.Build<T>(name, location, size),
            };
            var map = Mapping.CreateTypoToInspectorFieldMapping<T>(activeConfigurator);
            inspector.Modify.BuildFieldsForTypeByMapping(map);
            inspector.Binder.BindToTypo(map);
            return inspector;

            
        }

        public static InspectorController BuildInspector(string name, Point location, Size size)
        {
            var inspector = new InspectorController()
            {
                MyInspector = Inspector.Build(name, location, size),
            };
            return inspector;

            
        }
        private Form OwnerForm;
        public void AttatchToWindow(Form owner)
        {
            if(OwnerForm == null)
            {
                owner.Controls.Add(MyInspector.BackPanel);
                OwnerForm = owner;
            }
            
        }

    }
}
