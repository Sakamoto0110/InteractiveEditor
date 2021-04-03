using Editor.Options.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEditorControl.TestObjects
{
    [TypeSafeLock(true)]
    public class Moo
    {

        public int mooX = 0;
        public int mooY = 42;
        
        public Doo var_doo = new Doo();
        public int mooW = 53;
        public int mooH = 963;

    }
}
