using Editor.Events;
using Editor.Services.ServiceProvider;
using System;
using System.Reflection;
using static Editor.Modifiers;

namespace Editor.Services
{
    public class BindingService : IBindingService
    {
        public IOBServiceProvider Provider { get; set; }
        public InteractiveEditor Owner => Provider.Owner;


        public event EventHandler<BinderEventArgs> BindStarted;
        public event EventHandler<BinderEventArgs> BindFinished;

        public void BindToObject(object Instance, bool multiBind = false, Func<object, object, object, object> bruteForce = null)
        {
            var BinderEventArgs = new BinderEventArgs();
            BinderEventArgs.IsMultiBindEnabled = multiBind;
            BinderEventArgs.TargetInstance = Instance;
            BindStarted?.Invoke(Owner, BinderEventArgs);
            var actualPage = Owner;
            NEXT_PAGE:
            for (int i = 0; i < actualPage.Fields.Count; i++)
            {
                actualPage.Fields[i].BindToObject(Instance, multiBind, bruteForce ?? ((val, fs, cs) => val));
            }
            if (actualPage.NextPage != null)
            {
                actualPage = actualPage.NextPage;
                goto NEXT_PAGE;
            }

            BindFinished?.Invoke(Owner, BinderEventArgs);
        }
        public void UnbindObject()
        {
            for (int i = 0; i < Owner.Fields.Count; i++)
                Owner.Fields[i].Unbind();
        }
        public bool CheckIfFieldExists(string varName)
        {
            bool FieldExists = false;
            foreach (FieldInfo f in Owner.AvailableFields)
                if (f.Name.Equals(varName))
                {
                    FieldExists = true;
                    break;
                }
                else
                    FieldExists = false;
            return FieldExists;
        }
        public void BindToVariable(string key, string varName, CapFunction f = null, object capParms = null)
        {
            var isValidField = CheckIfFieldExists(varName);
            if (!isValidField)
            {
                Console.WriteLine("Failed to bind: " + varName);
                return;
            }
            var field = Provider.Request<FieldLocatorService>().LocateName(key);
            field?.BindToField(Owner.T, varName, f, capParms);

        }

    }
   
}
