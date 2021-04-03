using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Options.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MemberSafeLock : Attribute
    {
        public bool IsEnabled;
        
        public MemberSafeLock(bool v)
        {
            IsEnabled = v;
        }


    }
}
