using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyEditorControl
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DemoForm());
            //Application.Run(new MyWindow());
        }
        private struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
        [DllImport("Dwmapi.dll")]
        private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset );
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos( IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        const int GWL_EXSTYLE = -20;
        const uint WS_EX_LAYERED = 0x00080000;
        const uint WS_EX_TRANSPARENT = 0x00000020;

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly uint SWP_NOZORDER = 0x0200;

        public class MyWindow : ApplicationContext
        {
            private Form canvas;
            Timer t2;
            bool b = false;
            int c = 10;
            public MyWindow()
            {
                t2 = new Timer();
                t2.Interval = 1000;
                t2.Start();
                t2.Tick += T2_Tick;
                canvas = new Form();
                
                canvas.FormBorderStyle = FormBorderStyle.None;
                canvas.BackColor = Color.White;
                canvas.Location = new Point(0, 150);
                canvas.Size = new Size(50, 50);
                ///canvas.TransparencyKey = Color.White;
                canvas.Paint += Canvas_Paint;
                canvas.Opacity = 0.75f;
                canvas.Show();
                canvas.SetDesktopLocation(0, 0);
                Timer t = new Timer();
                t.Interval = 50;
                t.Start();
                t.Tick += T_Tick;


                IntPtr hwnd = canvas.Handle;
                var x = canvas.Location.X;
                var y = canvas.Location.Y;
                var w = canvas.Size.Width ;
                var h = canvas.Size.Height;
               
                SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
                SetWindowPos(hwnd, HWND_TOPMOST, x, y, w, h, SWP_NOZORDER);
            }

            private void T2_Tick(object sender, EventArgs e)
            {

                c--;
                if (c <= 0)
                {
                    uint SWP_NOSIZE = 0x0001;
                    uint SWP_SHOWWINDOW = 0x0040;
                    b = true;
                    t2.Tick -= T2_Tick;
                    IntPtr hwnd = canvas.Handle;
                    SetWindowPos(hwnd, HWND_TOPMOST, 0,0,0,0, SWP_NOSIZE);
                }
                    
            }

            private void T_Tick(object sender, EventArgs e)
            {
                canvas.Invalidate();
            }

            private void Canvas_Paint(object sender, PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.FillRectangle(new SolidBrush(Color.Magenta), new Rectangle(0, 0, 50, 50));
            }
        }
    }
}
