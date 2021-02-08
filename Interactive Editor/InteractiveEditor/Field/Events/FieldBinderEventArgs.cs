using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Fields.Events
{
    public class FieldBinderEventArgs : EventArgs
    {
        protected string _VariableFieldName = null;
        protected string _TargetFieldName = null;
        protected object _TargetInstance = null;
        protected object _FieldData = null;
        protected FieldInfo _FieldInfo = null;

        public string VariableFieldName
        {
            get => _VariableFieldName;
            set
            {
                if (_VariableFieldName == null)
                    _VariableFieldName = value;
            }
        }
        public string TargetFieldName
        {
            get => _TargetFieldName;
            set
            {
                if (_TargetFieldName == null)
                    _TargetFieldName = value;
            }
        }
        public object TargetInstance
        {
            get => _TargetInstance;
            set
            {
                if (_TargetInstance == null)
                    _TargetInstance = value;
            }
        }
        public object FieldData
        {
            get => _FieldData;
            set
            {
                if (_FieldData == null)
                    _FieldData = value;
            }
        }
        public FieldInfo FieldInfo
        {
            get => _FieldInfo;
            set
            {
                if (_FieldInfo == null)
                    _FieldInfo = value;
            }
        }

    }
}
