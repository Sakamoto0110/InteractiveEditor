using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Options.Attributes
{
    public class TypeSafeLock : Attribute
    {
        public enum FilterType
        {
            Blacklist = 0,
            Whitelist = 1
        }
        public bool IsEnabled;

        public string[] TargetFields;
        public FilterType _FilterType;

        public TypeSafeLock(bool v, string[] fieldNames = null, FilterType filterType = FilterType.Blacklist)
        {
            IsEnabled = v;
            _FilterType = filterType;
            TargetFields = fieldNames ?? new string[] { " " };

        }

    }
}
