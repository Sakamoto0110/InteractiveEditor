using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Fields.Events
{
    public class FieldChangedEventArgs : EventArgs
    {
        public int index;

        public FieldChangedEventArgs(int i)
        {
            index = i;
        }

    }
}
