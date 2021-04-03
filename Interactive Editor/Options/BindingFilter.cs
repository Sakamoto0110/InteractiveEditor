using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Options
{
    public enum FilterType
    {
        Undefined = 0,
        Blacklist = 1,
        Whitelist = 2,
        

    }
    public class BindingFilter
    {

        public string[] Values;
        public FilterType FilterType = FilterType.Undefined;

        public BindingFilter()
        {
            Values = Values ?? new[] { " " };
            FilterType = FilterType == FilterType.Undefined?FilterType.Blacklist:FilterType;
        }
    }
}
