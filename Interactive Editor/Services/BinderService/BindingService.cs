using Editor.Events;
using Editor.Fields;
using Editor.Services;
using Editor.Services.BinderService.Mapping;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using static Editor.Modifiers;

namespace Editor.Services
{
    

    public class BindingService : IBindingService
    {
        private readonly int MAX_NESTED_VARIABLE_LAYER = 10;
        enum FlterMode
        {
            undefined = 0,
            Insert = 1,
            Subtract = 2,
        }
        public IOBServiceProvider Provider { get; set; }
        public Inspector Owner => Provider.Owner;


        public event EventHandler<BinderEventArgs> BindStarted;
        public event EventHandler<BinderEventArgs> BindFinished;
        
        
        public delegate void BindingConfigurator(MapHandler mapping);
        public delegate void PostBindingConfigurator(Fieldset fieldset);
        
        public void BindToObject(object Instance, bool multiBind = false, Func<object, object, object, object> bruteForce = null)
        {
            var BinderEventArgs = new BinderEventArgs();
            BinderEventArgs.IsMultiBindEnabled = multiBind;
            BinderEventArgs.TargetInstance = Instance;
            BindStarted?.Invoke(Owner, BinderEventArgs);

         
            Owner.Invoker.InvokeForAllFields((fs) => fs.BindToObject(Instance, multiBind, bruteForce ?? ((val, f, cs) => val)) );


            BindFinished?.Invoke(Owner, BinderEventArgs);
        }
        public void UnbindObject()
        {
            Owner.Invoker.InvokeForAllFields((fs) => fs.Unbind());
            
        }
        public bool CheckIfFieldExists(string varName) 
        {
            int n = 0;
            
            try
            {
                return CheckIfFieldExistsInType(Owner.T);
            }catch(StackOverflowException ex)
            {                
                Console.WriteLine(ex);
                return false;
            }
            




            bool CheckIfFieldExistsInType(Type type)
            {
                n++;
                if(n > MAX_NESTED_VARIABLE_LAYER)                
                    throw new StackOverflowException("Max nested layers reached!");
                
                bool FieldExists = false;
                var avalTypes = type.GetFields();
                
                for (int i = 0; i < avalTypes.Length; i++)
                {
                    FieldInfo f = avalTypes[i];

                    if (AvalClass(f.FieldType))
                    {
                        FieldExists = CheckIfFieldExistsInType(f.FieldType);
                        if (FieldExists)
                            break;
                    }
                    if (f.Name.Equals(varName))
                    {
                        FieldExists = true;
                        break;
                    }
                    else
                    {
                        FieldExists = false;
                    }
                }
                



                return FieldExists;
            }
                
                    
        }
        public void BindToVariable(string key, string varName, CapFunction f = null, object capParms = null)
        {
            var isValidField = CheckIfFieldExists(varName);
            if (!isValidField)
            {
                Console.WriteLine("Failed to bind: " + varName);
                return;
            }
            if(key is null)
            {
                var field = Provider.Request<FieldLocatorService>().LocateLast();
                field?.BindToField(Owner.T, varName, f, capParms);
            }
            else
            {
                var field = Provider.Request<FieldLocatorService>().LocateName(key);
                field?.BindToField(Owner.T, varName, f, capParms);
            }
        }


        public void BindToTypo(MapObject map, BindingConfigurator activeConfigurator = null)
        {
           
            DoBind(map.Map);
        }

       














      
        
        
        internal void DoBind(Dictionary<string, BindingArgs> map)
        {
            

            int _i = 0;
            var keyArr = new string[map.Keys.Count];
            foreach (string str in map.Keys)
            {
                keyArr[_i] = str;
                _i++;
            }
            for(int i = 0; i < keyArr.Length; i++)            
            {
                string str = keyArr[i];
                BindingArgs bindingArgs = map[str];
                Fieldset fieldset = Provider.Request<FieldLocatorService>().LocateName(bindingArgs.FieldSet_Name);
                
                if (bindingArgs.IsGroup)
                {
                    Type localType = bindingArgs.TargetVariable_Type;
                    
                    fieldset.Field.Visible = false;                                                          
                    fieldset.Label.Font = new Font(fieldset.Label.Font, FontStyle.Bold);

                    //fieldset.QuestionMark.Visible = false;
                    fieldset.GroupSize = bindingArgs.GroupSize;
                    fieldset.IsGroupOwner = true;
                    fieldset.SetAsCollapseable();


                    for (int j = 1; j < bindingArgs.GroupSize+1; j++)
                    {
                        var targetName = bindingArgs.GroupMembers[j - 1];
                        var fs = Owner.LocateField.LocateName(targetName);
                        fieldset.Children.Add(fs);
                        fieldset.GroupMembersName.Add(targetName);

                        for (int k = 0; k < fieldset.GroupFieldInfo.Count; k++)
                            fs.GroupFieldInfo.Add(fieldset.GroupFieldInfo[k]);
                        fs.GroupFieldInfo.Add(bindingArgs.TargetVariable_FieldInfo);

                        var args = map[keyArr[i + j]];
                        ExecuteBind(args.FieldSet_Name, args.TargetVariable_Name, args.capFunction,args.capParms);
    
                        
                        // Fix posX of fields/labels
                        var offX = (fs.GroupFieldInfo.Count * 10);
                        fs.BackPanel.Location = new Point(fs.BackPanel.Location.X + offX, fs.BackPanel.Location.Y);
                        fs.BackPanel.Width -= offX;
                        fs.Field.Location = new Point(fs.Field.Location.X - offX, fs.Field.Location.Y);
                        //fieldset.BackPanel.Size = new Size(fieldset.BackPanel.Width - offX, fieldset.BackPanel.Height);
                        //fieldset.BackPanel.Location = new Point(fs.BackPanel.Location.X, fieldset.BackPanel.Location.Y);
                    }

                }
                else
                {
                    var args = bindingArgs;
                    ExecuteBind(args.FieldSet_Name, args.TargetVariable_Name, args.capFunction, args.capParms);
                }

                if (fieldset.Field  is ComboBox cb)
                {

                    cb.SelectedIndex = -1;
                }
                
                bindingArgs.Post?.Invoke(fieldset);
            }
        }
        
        internal void ExecuteBind(string key, string varName, CapFunction capFunction, object capParms)
        {
            Owner.Binder.BindToVariable(key,varName,capFunction,capParms);
        }

        internal void DefaultPost()
        {
            Owner.Invoker.InvokeForAllFields((fs) =>
            {
                if (fs.Field is ComboBox cb)
                {
                    cb.SelectedIndex = -1;
                }
            });
            



        }

        internal bool AvalClass(Type t)
        {
            return t.IsClass &&
                   t != typeof(string) &&
                   t != typeof(String) &&
                   t != typeof(Type);

        }

        internal Type DefaultControlMapping(Type targetType)
        {
            if (targetType.Name ==  typeof(String).Name)
                return typeof(TextBox);
            if (targetType.IsEnum)
                return typeof(ComboBox);
           // if (targetType.IsClass)            
            //    return typeof(Misc.Separator);
            
                
            return typeof(TextBox);
        }
      
    }
   
}
