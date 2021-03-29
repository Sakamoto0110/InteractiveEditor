using Editor.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Fields.Events
{
    public class FieldBinderFailureEventArgs : FieldBinderEventArgs
    {
        public int Line { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
        public string Reasson { get; set; }
        public string PossibleSolution { get; set; }
        public List<Exception> Exceptions { get; } = new List<Exception>();
        public override string ToString()
        {
            return $"[{VariableFieldName}] {Message}. Reason: {Reasson}. Caused by: {Source} at Line: {Line}.   Possible solution: {PossibleSolution}";

        }
    }
   
}
