using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editor.Options
{
    public class InspectorOptions
    {
        public bool CreateOwnWindow = false;
        public DockStyle DockStyle = DockStyle.None;

        public string Name;

        public Point Location;
        public Size Size;
        public Padding Margins = new Padding(5, 15, 15, 0);
        
        public int VerticalSpacing;
        public int FieldHeight;

        public int HeaderHeight;
        public int FooterHeight;

        public bool AutoWidth = true;
        public bool ShowText = true;

        public BindingFilter BindingFilter;

        public bool CanCollapse = true;
        public int NestedTypeXOffset = 15;

    }
}
