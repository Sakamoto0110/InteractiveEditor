using Editor.Events;
using Editor.Fields;
using Editor.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using static Editor.Modifiers;

namespace Editor.Services
{
    public struct PreBindingArgs
    {
        public PreBindingArgs(PreBindingArgs other)
        {
            DoBind = other.DoBind;
            FieldSet_Name = other.FieldSet_Name;
            FieldSet_FieldFlags = other.FieldSet_FieldFlags;
            FieldSet_FieldType = other.FieldSet_FieldType;
            TargetVariable_Name = other.TargetVariable_Name;
            TargetVariable_Type = other.TargetVariable_Type;
            TargetVariable_FieldInfo = other.TargetVariable_FieldInfo;
            Post = other.Post;
            IsGroup = other.IsGroup;
            GroupSize = other.GroupSize;
        }
        public bool DoBind;
        public string FieldSet_Name;
        public FieldFlags FieldSet_FieldFlags;
        public Type FieldSet_FieldType;
        public Type TargetVariable_Type;
        public string TargetVariable_Name;
        public FieldInfo TargetVariable_FieldInfo;
        public BindingService.PostBindingConfigurator Post;

        public bool IsGroup;
        public int GroupSize;

    }
    // Search flags
    //
    // all 
    // only_public
    // public_and_protected
    // require_attribute
    //
    //
    //
    //
    //
    public enum FilterMode
    {
        Blacklist = -1,
        Whitelist = 1,
    }

    public class BindingService : IBindingService
    {
        enum FlterMode
        {
            undefined = 0,
            Insert = 1,
            Subtract = 2,
        }
        public IOBServiceProvider Provider { get; set; }
        public InteractiveEditor Owner => Provider.Owner;


        public event EventHandler<BinderEventArgs> BindStarted;
        public event EventHandler<BinderEventArgs> BindFinished;
        
        public delegate void BindingConfigurator(ref Dictionary<string, PreBindingArgs> mapping);
        public delegate void PostBindingConfigurator(Fieldset fieldset);

        public void BindToObject(object Instance, bool multiBind = false, Func<object, object, object, object> bruteForce = null)
        {
            var BinderEventArgs = new BinderEventArgs();
            BinderEventArgs.IsMultiBindEnabled = multiBind;
            BinderEventArgs.TargetInstance = Instance;
            BindStarted?.Invoke(Owner, BinderEventArgs);

         
            Owner.Invoker.InvokeForAllFieldsEx((fs) => fs.BindToObject(Instance, multiBind, bruteForce ?? ((val, f, cs) => val)) );


            BindFinished?.Invoke(Owner, BinderEventArgs);
        }
        public void UnbindObject()
        {
            Owner.Invoker.InvokeForAllFieldsEx((fs) => fs.Unbind());
            
        }
        public bool CheckIfFieldExists(string varName)
        {
            return CheckIfFieldExistsInType(Owner.T);

            bool CheckIfFieldExistsInType(Type type)
            {
                
                bool FieldExists = false;
                var avalTypes = type.GetFields();
                for (int i = 0; i < avalTypes.Length; i++)
                {
                    FieldInfo f = avalTypes[i];
                    
                    if (f.FieldType.IsClass)
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
        
     


        public void BindToTypo(string[] targetFilter = null, FilterMode filterOperation = FilterMode.Blacklist)
        {
            var filtredVars = ApplyFilter(targetFilter, filterOperation);
            var mappedVars = ApplyMapping(filtredVars);
            var typeResolvedVars = ResolveTypes(mappedVars);
            DoBind(typeResolvedVars);
            DefaultPost();
        }
        internal void DefaultPost()
        {
           Owner.Invoker.InvokeForAllFieldsEx((fs) =>
           {
               if(fs.Field is ComboBox cb)
               {
                   cb.SelectedIndex = -1;
               }
           });
            var actualPage = Owner;
            for (int i = 0; i < Owner.LocatePage.Count; i++)
            {
                // Owner.LocatePage.LocateLast().OnPageDown(null, null);
                Console.WriteLine(actualPage.Fields[0].Name);
                if (actualPage.PrevPage != null)
                    actualPage.Visible = false;
                
                actualPage = actualPage.NextPage??actualPage;
                
            }

            Owner.GoToFirstPage();

            

        }
        public void BindToTypo(BindingConfigurator FTypeToControlMapping, string[] targetFilter = null, FilterMode filterOperation = FilterMode.Blacklist)
        {
            var filtredVars = ApplyFilter(targetFilter, filterOperation);
            var mappedVars  = ApplyMapping(filtredVars, FTypeToControlMapping);
            var typeResolvedVars = ResolveTypes(mappedVars);
            DoBind(typeResolvedVars);
            DefaultPost();
            // Filter names                                 ( ApplyFilter    ) |
            // Map variabe type into fieldset               ( PreResolveType ) | 
            // Create field of right type                   ( ResolveType    ) V

        }
        internal FieldInfo[] ApplyFilter(string[] targetFilter, FilterMode op)
        {
            var oldTargetList = new List<FieldInfo>(Owner.T.GetFields());
            var newTargetList = new List<FieldInfo>();
            if(targetFilter is null)
                targetFilter = new string[] { "" };
            switch (op)
            {
                case FilterMode.Whitelist:
                    foreach (FieldInfo finfo in oldTargetList)                     
                        foreach (string fieldinfo in targetFilter)                        
                            if (finfo.Name.Equals(fieldinfo))
                                newTargetList.Add(finfo);                                            
                                                                                                          
                    break;
                case FilterMode.Blacklist:
                    foreach (FieldInfo finfo in oldTargetList)
                    {
                        bool blacklisted = false;
                        foreach (string fieldinfo in targetFilter)
                        {
                            if (fieldinfo.Equals(finfo.Name))
                                blacklisted = true;
                        }
                        if(!blacklisted)
                            newTargetList.Add(finfo);

                    }
                        
                    break;
            }
            
            int c = newTargetList.Count;
            for (int i = 0; i < c; i++)
            {
                FieldInfo fi = newTargetList[i];
                if (fi.FieldType.IsClass)
                {
                    List<FieldInfo> ToAdd = new List<FieldInfo>();
                    AddNestedMembers(fi, ToAdd);
                    newTargetList.InsertRange(i + 1, ToAdd);
                    
                        
                }
            }
            
            return newTargetList.ToArray();

            void AddNestedMembers(FieldInfo finfo, List<FieldInfo> _list)
            {
                FieldInfo[] nestedTypeInfos = finfo.FieldType.GetFields();
                for (int j = 0; j < nestedTypeInfos.Length; j++)
                {
                    var ftype = nestedTypeInfos[j].FieldType;
                    _list.Add(nestedTypeInfos[j]);
                    if (ftype.IsClass)
                    {
                        AddNestedMembers(nestedTypeInfos[j], _list);
                    }


                }
            }
        }
        
        internal Dictionary<string, PreBindingArgs> ApplyMapping(FieldInfo[] target, BindingConfigurator FTypeToControlMapping = null)
        {
            Dictionary<string, PreBindingArgs> map = new Dictionary<string, PreBindingArgs>();
            foreach (FieldInfo finfo in target)
            {
                PreBindingArgs args = new PreBindingArgs
                {
                    TargetVariable_Type = finfo.FieldType,
                    TargetVariable_Name = finfo.Name,
                    TargetVariable_FieldInfo = finfo,
                    FieldSet_Name = finfo.Name,
                    FieldSet_FieldType = null,                    
                    FieldSet_FieldFlags = FieldFlags.None,
                    DoBind = true,
                    Post = null,
                    
                };
                if (!map.ContainsKey(finfo.Name))
                {
                    map.Add(finfo.Name, args);
                }
                
            }
            FTypeToControlMapping?.Invoke(ref map);            
            return map;
        }


        internal Dictionary<string, PreBindingArgs> ResolveTypes(Dictionary<string, PreBindingArgs> map)
        {
            int _i = 0;
            var keyArr = new string[map.Keys.Count];
            foreach (string str in map.Keys)
            {
                keyArr[_i] = str;
                _i++;
            }
            for (int i = 0; i < keyArr.Length; i++)
            {
                string str = keyArr[i];
                PreBindingArgs bindingArgs = map[str];
                if (!bindingArgs.DoBind)
                    continue;
                
                if (bindingArgs.FieldSet_FieldType is null)                
                    bindingArgs.FieldSet_FieldType = DefaultControlMapping(bindingArgs.TargetVariable_Type);
                                    
                if (bindingArgs.FieldSet_FieldType == typeof(ComboBox))
                {
                    Owner.Modify.AddField<ComboBox>(bindingArgs.FieldSet_Name, bindingArgs.FieldSet_FieldFlags);
                    ComboBox comboBox = Owner.Modify.EditField() as ComboBox;
                    comboBox.DataSource = Enum.GetValues(bindingArgs.TargetVariable_Type);
                    comboBox.SelectedIndex = -1;
                    comboBox.SelectedItem = null;
                    //Owner.Binder.BindToVariable(null, bindingArgs.TargetVariable_Name);
                    continue;
                }

                if (bindingArgs.TargetVariable_Type.IsClass)
                {
                    int gsize = 0;
                    foreach(FieldInfo innerInfo in bindingArgs.TargetVariable_Type.GetFields())                    
                        foreach (string innerStr in map.Keys)                        
                            if (innerInfo.Name.Equals(innerStr))                            
                                gsize++;                    
                    if(gsize > 0)
                    {                        
                        bindingArgs.IsGroup = true;
                        bindingArgs.GroupSize = gsize;
                        map[str] = new PreBindingArgs(bindingArgs);
                        
                    }
                }
                Owner.Modify.AddField(bindingArgs.FieldSet_FieldType, bindingArgs.FieldSet_Name, bindingArgs.FieldSet_FieldFlags);

            }

            return map;
        }

        
        internal void DoBind(Dictionary<string, PreBindingArgs> map)
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
                PreBindingArgs bindingArgs = map[str];
                Fieldset fieldset = Provider.Request<FieldLocatorService>().LocateName(bindingArgs.FieldSet_Name);
                
                if (bindingArgs.IsGroup)
                {
                    Type localType = bindingArgs.TargetVariable_Type;
                    for(int j = 1; j <= bindingArgs.GroupSize; j++)
                    {
                        var fs = Owner.LocateField.LocateName(map[keyArr[i + j]].FieldSet_Name);                        
                        for (int k = 0; k < fieldset.GroupFieldInfo.Count; k++)
                            fs.GroupFieldInfo.Add(fieldset.GroupFieldInfo[k]);

                        fs.GroupFieldInfo.Add(bindingArgs.TargetVariable_FieldInfo);

                        Owner.Binder.BindToVariable(map[keyArr[i + j]].FieldSet_Name, map[keyArr[i + j]].TargetVariable_Name);
                       
                        // Fix posX of fields/labels
                        var offX = (fs.GroupFieldInfo.Count * 10);
                        fs.BackPanel.Location = new Point(fs.BackPanel.Location.X + offX, fs.BackPanel.Location.Y);
                        fs.BackPanel.Width -= offX;
                        fs.Field.Location = new Point(fs.Field.Location.X - offX, fs.Field.Location.Y);
                    }

                    continue;
                }
                else
                {
                    Owner.Binder.BindToVariable(bindingArgs.FieldSet_Name, bindingArgs.TargetVariable_Name);
                }

                if (fieldset.Field  is ComboBox cb)
                {

                    cb.SelectedIndex = -1;
                }
                
                bindingArgs.Post?.Invoke(Owner.LocateField.LocateLast());
            }
        }

        internal Type DefaultControlMapping(Type targetType)
        {
            if (targetType.IsEnum)
                return typeof(ComboBox);
            if (targetType.IsClass)            
                return typeof(Misc.Separator);
            
                
            return typeof(TextBox);
        }
      
    }
   
}
