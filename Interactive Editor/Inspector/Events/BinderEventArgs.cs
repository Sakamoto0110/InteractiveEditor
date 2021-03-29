using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Events
{
    public class BinderEventArgs : EventArgs
    {
        protected object _TargetInstance = null;

        public bool IsMultiBindEnabled { get; set; }
        public object TargetInstance
        {
            get => _TargetInstance;
            set
            {
                if (_TargetInstance == null)
                    _TargetInstance = value;
            }
        }
        public string[] VariableFieldsNames { get; set; }
    }
}
