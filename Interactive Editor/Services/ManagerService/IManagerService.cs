using Editor.Fields;
using Editor.Misc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Editor.Modifiers;

namespace Editor.Services
{
    public interface IManagerService
    {
        string Name { get; set; }

        Point Location { get; set; }

        Size Size { get; set; }


        List<Fieldset> Fields { get; }

        Panel BackPanel { get; }

        bool Visible { get; set; }

        bool ShowText { get; set; }

        bool AutoSize { get; set; }

        int Horizontal_Spacing { get; set; }

        int FieldHeight { get; set; }

        Padding Margins { get; set; }

        bool AutoUpdateEnabled { get; set; }

        ControlFlags CFlags { get; set; }

        int MiscControlHeight { get; set; }

    }
}
