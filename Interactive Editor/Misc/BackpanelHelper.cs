using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editor.Misc
{
    public class BackpanelHelper : GroupBox
    {
        public bool MYVARTEST;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg,
                                             bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;
        private const int WM_PAINT = 0xf;
        private const int WM_CREATE = 0x1;

        public BackpanelHelper()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer,true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ContainerControl, true);
        }

        public static void SuspendDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_PAINT, false, 0);
        }

        public static void ResumeDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_PAINT, true, 0);
            parent.Refresh();
        }

        
    }
}
