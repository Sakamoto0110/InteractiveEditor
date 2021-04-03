using Editor.Options;
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

        private Form OwnerForm;

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
        
        public static InspectorController BuildInspector<T>(InspectorOptions options, BindingConfigurator activeConfigurator)
        {
            var inspector = new InspectorController()
            {
                MyInspector = Inspector.Build<T>(options.Name, options.Location, options.Size, opt: options),
            };
            var map = Mapping.CreateTypoToInspectorFieldMapping<T>(activeConfigurator, options.BindingFilter);
            inspector.Modify.BuildFieldsForTypeByMapping(map);
            inspector.Binder.BindToTypo(map);
            if (options.CreateOwnWindow)
            {
                inspector.AttatchToWindow(new Form() 
                {
                    Visible = true,
                    Text = $"Inspecting: \"{options.Name}\" ",
                    ShowIcon = false,

                });
                inspector.MyInspector.BackPanel.Dock = options.DockStyle;
            }
            return inspector;
        }

        public void AttatchToWindow(Form owner)
        {
            if(OwnerForm == null)
            {
                MyInspector.OwnerForm = owner;
                owner.Controls.Add(MyInspector.BackPanel);
                OwnerForm = owner;
            }
            else
            {
                Console.WriteLine("Owner window already exists, dettatch old window before attatching to a new window.");
            }
            
        }

    }
}
