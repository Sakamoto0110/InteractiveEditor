using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Services.BinderService.Mapping
{
    public class MapHandler
    {
        public Dictionary<string, BindingArgs> Map;

        public delegate void ModFunction(ref BindingArgs args);

        public MapHandler(Dictionary<string, BindingArgs> mapping)
        {
            Map = mapping;
        }


        public void Modify(string key, ModFunction modFunc)
        {
            if (!Map.ContainsKey(key))
                throw new ArgumentException($"Key does not exists [{key}]");
            Map.TryGetValue(key, out BindingArgs args);
            modFunc(ref args);
            Map[key] = new BindingArgs(args);
        }

    }
}
