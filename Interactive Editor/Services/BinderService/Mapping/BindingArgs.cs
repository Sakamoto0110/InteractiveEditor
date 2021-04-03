using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Editor.Modifiers;
using static Editor.Services.BindingService;

namespace Editor.Services.BinderService.Mapping
{
    public class BindingArgs
    {
        public BindingArgs() { }
        public BindingArgs(BindingArgs other)
        {
            DoBind = other.DoBind;
            FieldSet_Name = other.FieldSet_Name;
            FieldSet_FieldType = other.FieldSet_FieldType;
            TargetVariable_Name = other.TargetVariable_Name;
            TargetVariable_Type = other.TargetVariable_Type;
            TargetVariable_FieldInfo = other.TargetVariable_FieldInfo;
            Post = other.Post;
            IsGroup = other.IsGroup;
            GroupSize = other.GroupSize;
            capFunction = other.capFunction;
            capParms = other.capParms;
            FieldSet_Text = other.FieldSet_Text;
            GroupMembers = new List<string>(other.GroupMembers ?? new List<string>() { " " });
            FieldFlags = other.FieldFlags;
        }

        public bool DoBind;

        public string FieldSet_Name;
        public string FieldSet_Text;
        public Type FieldSet_FieldType;
        public FieldFlags FieldFlags;

        public Type TargetVariable_Type;
        public string TargetVariable_Name;
        public FieldInfo TargetVariable_FieldInfo;   
        
        public PostBindingConfigurator Post;
        
        public bool IsGroup;
        public int GroupSize;
        public List<string> GroupMembers;

        public CapFunction capFunction;
        public object capParms;

    }
}
