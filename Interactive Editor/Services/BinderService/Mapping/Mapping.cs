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


        internal static FieldInfo[] ApplyFilter(Type T)
        {
            var newTargetList = new List<FieldInfo>(T.GetFields());


            int c = newTargetList.Count;
            int maxTries = 20;
            for (int i = 0; i < c; i++)
            {
                FieldInfo fi = newTargetList[i];
                if (AvailClass(fi.FieldType))
                {
                    List<FieldInfo> ToAdd = new List<FieldInfo>();
                    AddNestedMembers(fi, ToAdd, maxTries);
                    newTargetList.InsertRange(i + 1, ToAdd);


                }
            }

            return newTargetList.ToArray();

            void AddNestedMembers(FieldInfo finfo, List<FieldInfo> _list, int tries)
            {
                maxTries--;
                FieldInfo[] nestedTypeInfos = finfo.FieldType.GetFields();
                for (int j = 0; j < nestedTypeInfos.Length; j++)
                {
                    var ftype = nestedTypeInfos[j].FieldType;
                    _list.Add(nestedTypeInfos[j]);
                    if (AvailClass(nestedTypeInfos[j].FieldType))
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
            foreach(BindingArgs args in map.Values)
            {
                Console.WriteLine(args.FieldSet_Name + " " + args.FieldSet_Text);
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

                if (AvailClass(bindingArgs.TargetVariable_Type))
                {
                    int gsize = 0;
                    foreach (FieldInfo innerInfo in bindingArgs.TargetVariable_Type.GetFields())
                        foreach (string innerStr in map.Keys)
                            if (innerInfo.Name.Equals(innerStr))
                                gsize++;
                    if (gsize > 0)
                    {
                        bindingArgs.IsGroup = true;
                        bindingArgs.GroupSize = gsize;
                        map[str] = new BindingArgs(bindingArgs);

                    }
                }


            }
            return map;
        }







        public static bool AvailClass(Type t)
        {
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





