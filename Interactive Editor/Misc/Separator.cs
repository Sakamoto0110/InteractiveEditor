using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editor.Misc
{
    public class Separator : GroupBox
    {
        public Separator()
        {
            this.Height = 3;
            this.Width = 300;
            this.Text = "";
            this.Location = new System.Drawing.Point(0, 10);
            this.Visible = false;
        }


    }
}
