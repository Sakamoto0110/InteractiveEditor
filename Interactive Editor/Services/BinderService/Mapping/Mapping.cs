using Editor.Options;
using Editor.Options.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Editor.Services.BindingService;

namespace Editor.Services.BinderService.Mapping
{

    public class MapObject 
    {
        public Dictionary<string, BindingArgs> Map;

        public Dictionary<string, BindingArgs>.ValueCollection Values
        {
            get => Map.Values;
        }
        public Dictionary<string, BindingArgs>.KeyCollection Keys
        {
            get => Map.Keys;
        }

        public MapObject(Dictionary<string, BindingArgs> map) 
        {
            Map = map;            
        }

    
    }
    public class Mapping
    {


        public static MapObject CreateTypoToInspectorFieldMapping<T>(BindingConfigurator configurator)
        {
            var filtredVars = ApplyFilter(typeof(T));
            var mappedVars = ApplyMapping(filtredVars, configurator);
            var typeResolvedVars = ResolveTypes(mappedVars);
            return new MapObject(typeResolvedVars);
        }


        public static MapObject CreateTypoToInspectorFieldMapping<T>(BindingConfigurator configurator, BindingFilter filter = null)
        {
            filter = filter ?? new BindingFilter();
            if (GlobalOptions.RootRequireAttrTypeSafeLock || GlobalOptions.AlwaysRequireAttrTypeSafeLock)
            {
                var attr = typeof(T).GetCustomAttribute(typeof(TypeSafeLock));

                if (attr is TypeSafeLock tsl)
                {
                    filter.Values = tsl.TargetFields;
                }
                else
                {
                    Console.WriteLine("TypeSafeLock attribute is required");
                    throw new Exception();
                }
            }

            var filtredVars = ApplyFilter(typeof(T), filter);
            var mappedVars = ApplyMapping(filtredVars, configurator);
            var typeResolvedVars = ResolveTypes(mappedVars);
            return new MapObject(typeResolvedVars);
        }


        internal static FieldInfo[] ApplyFilter(Type T, BindingFilter filter = null)
        {
            filter = filter ?? new BindingFilter() { Values = new[] { "" }, FilterType = FilterType.Blacklist };
            var newTargetList = new List<FieldInfo>(T.GetFields());




            if (filter.FilterType.ToString().Equals("Blacklist"))
                ApplyBlacklist();
            else if(filter.FilterType.ToString().Equals("Whitelist"))
                ApplyWhitelist();
            
            
                
            


            int c = newTargetList.Count;
            int maxTries = 200;
            for (int i = 0; i < c; i++)
            {
                FieldInfo fi = newTargetList[i];
                if (AvailClass(fi) )
                {
                    List<FieldInfo> ToAdd = new List<FieldInfo>();
                    AddNestedMembers(fi, ToAdd, maxTries);
                    newTargetList.InsertRange(i + 1, ToAdd);

                }
            }

            for (int i = 0; i < newTargetList.Count; i++)
                if (!CheckTypeSafeLockAttr(newTargetList[i]))
                    newTargetList.RemoveAt(i--);

            return newTargetList.ToArray();

            void ApplyBlacklist()
            {
                for (int c2 = 0; c2 < newTargetList.Count; c2++)
                    foreach (string s in filter.Values)
                        if (newTargetList[c2].Name.Equals(s))
                            newTargetList.RemoveAt(c2--);
            }
            void ApplyWhitelist()
            {
                for (int c2 = 0; c2 < newTargetList.Count; c2++)
                {
                    bool isInWhitelist = false;
                    foreach (string s in filter.Values)
                        if (newTargetList[c2].Name.Equals(s))
                            isInWhitelist = true;
                    if (!isInWhitelist)
                        newTargetList.RemoveAt(c2--);

                }
            }

            void AddNestedMembers(FieldInfo finfo, List<FieldInfo> _list, int tries)
            {
                maxTries--;
                FieldInfo[] nestedTypeInfos = finfo.FieldType.GetFields();
                for (int j = 0; j < nestedTypeInfos.Length; j++)
                {
                    var ftype = nestedTypeInfos[j].FieldType;
                    _list.Add(nestedTypeInfos[j]);

                    if (AvailClass(nestedTypeInfos[j]))
                    {
                        if (tries >= 0)
                            AddNestedMembers(nestedTypeInfos[j], _list, tries - 1);
                    }


                }
            }
        }




        internal static Dictionary<string, BindingArgs> ApplyMapping(FieldInfo[] target, BindingConfigurator FTypeToControlMapping)
        {
            Dictionary<string, BindingArgs> map = new Dictionary<string, BindingArgs>();
            foreach (FieldInfo finfo in target)
            {
                BindingArgs args = new BindingArgs()
                {
                    TargetVariable_Type = finfo.FieldType,
                    TargetVariable_Name = finfo.Name,
                    TargetVariable_FieldInfo = finfo,
                    FieldSet_Name = finfo.Name,
                    FieldSet_FieldType = null,
                    DoBind = true,
                };
                if (!map.ContainsKey(finfo.Name))
                {
                    map.Add(finfo.Name, args);
                }

            }
            
            try
            {

                FTypeToControlMapping?.Invoke(new MapHandler(map));

            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong during custom mapping");
                Console.WriteLine(ex.Message);
            }

            return map;
        }


        internal static Dictionary<string, BindingArgs> ResolveTypes(Dictionary<string, BindingArgs> map)
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
                BindingArgs bindingArgs = map[str];
                if (!bindingArgs.DoBind)
                    continue;

                if (bindingArgs.FieldSet_FieldType is null)
                    bindingArgs.FieldSet_FieldType = DefaultControlMapping(bindingArgs.TargetVariable_Type);

                if (bindingArgs.TargetVariable_Type.IsEnum)
                {

                    continue;
                }

                if (AvailClass(bindingArgs.TargetVariable_FieldInfo))
                {
                    int gsize = 0;
                    List<string> members = new List<string>();
                    foreach (FieldInfo innerInfo in bindingArgs.TargetVariable_Type.GetFields())
                        foreach (string innerStr in map.Keys)
                            if (innerInfo.Name.Equals(innerStr))
                            {
                                gsize++;                                                                
                                members.Add(innerStr);
                            }
                                
                    if (gsize > 0)
                    {
                        bindingArgs.GroupMembers = new List<string>(members);
                        bindingArgs.IsGroup = true;
                        bindingArgs.GroupSize = gsize;
                        map[str] = new BindingArgs(bindingArgs);
                    }
                }


            }
            return map;
        }




        internal static bool CheckTypeSafeLockAttr(FieldInfo fi)
        {
            Type t = fi.FieldType;
            if (t.IsClass && GlobalOptions.AlwaysRequireAttrTypeSafeLock)
            {
                var tsl = t.GetCustomAttribute<TypeSafeLock>();
                if (tsl == null)
                {
                    Console.WriteLine($"Missing TypeSafeAttr at: {fi.Name}[{fi.FieldType}] ");
                    return false;
                }
                else if (!tsl.IsEnabled)
                {
                    Console.WriteLine($"TypeSafeAttr is locked at: {fi.Name}[{fi.FieldType}] ");
                    return false;
                }
            }


            return true;
        }


        public static bool AvailClass(FieldInfo fi)
        {
            Type t = fi.FieldType;
            if (!CheckTypeSafeLockAttr(fi))
                return false;
            return t.IsClass &&
                   t != typeof(string) &&
                   t != typeof(String) &&
                   t != typeof(Type);

        }

        internal static Type DefaultControlMapping(Type targetType)
        {
            if (targetType.Name == typeof(String).Name)
                return typeof(TextBox);
            if (targetType.IsEnum)
                return typeof(ComboBox);
            // if (targetType.IsClass)            
            //    return typeof(Misc.Separator);


            return typeof(TextBox);
        }





    }


}





