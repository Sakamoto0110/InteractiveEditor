using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Editor;
using Editor.Fields;
using Editor.Services;
using Editor.Services.LocatorService;

namespace MyEditorControl
{
    public partial class DemoForm : Form
    {
        public InteractiveEditor Editor;
        public Foo MyFoo;
        public Foo MyFoo2;
        public Foo MyFoo3;
        public Foo MyFoo4;
        public DemoForm()
        {
            
            MyFoo = new Foo(360 - 25, 360 - 25,50,50);
            MyFoo2 = new Foo(200+360 - 25, 360 - 25, 50, 50);
            MyFoo3 = new Foo(200 + 360 - 25,100+ 360 - 25, 50, 50);
            MyFoo4 = new Foo( 360 - 25, 100+360 - 25, 50, 50);
            InitializeComponent();
            Editor = InteractiveEditor.GenerateMyEditor<Foo>(this, "name", 10, 10, 200, 230, Modifiers.ControlFlags.EnablePages);
            Editor.Visible = true;
            Controls.Add(Editor.BackPanel);
            this.Size = new Size(720, 720);
            this.Paint += Form1_Paint;
            Editor.Manipulator.AddField<TextBox>("Foo X");
            Editor.Manipulator.AddField<TextBox>("Foo Y");
            Editor.Manipulator.AddField<TextBox>("Foo W");
            Editor.Manipulator.AddField<TextBox>("Foo H");

            Editor.Manipulator.AddField<TextBox>("Foo A");
            Editor.Manipulator.AddField<TextBox>("Foo B");
            Editor.Manipulator.AddField<TextBox>("Foo C");
            Editor.Manipulator.AddField<TextBox>("Foo D");

            Editor.Manipulator.AddField<TextBox>("Foo G");
            Editor.Manipulator.AddField<TextBox>("Foo R");
            Editor.Manipulator.AddField<TextBox>("Foo J");
            Editor.Manipulator.AddField<TextBox>("Foo L");

            Editor.Manipulator.AddField<TextBox>("Foo U");
            Editor.Manipulator.AddField<TextBox>("Foo I");
            Editor.Manipulator.AddField<TextBox>("Foo O");
            Editor.Manipulator.AddField<TextBox>("Foo P");

            Editor.Manipulator.AddField<TextBox>("Foo 9");
            Editor.Manipulator.AddField<TextBox>("Foo 5");
            Editor.Manipulator.AddField<TextBox>("Foo 8");
            Editor.Manipulator.AddField<TextBox>("Foo 7");

            Editor.Manipulator.AddField<TextBox>("Foo 1");
            Editor.Manipulator.AddField<TextBox>("Foo 2");
            Editor.Manipulator.AddField<TextBox>("Foo 3");
            Editor.Manipulator.AddField<TextBox>("Foo 4");

            
            

            //Editor.AddField<TextBox>("Foo I");
            //Editor.AddField<TextBox>("Foo J");
            //Editor.AddField<TextBox>("Foo K");
            //Editor.AddField<TextBox>("Foo U");

            Editor.Binder.BindToVariable("Foo X", "x");
            Editor.Binder.BindToVariable("Foo Y", "y");
            Editor.Binder.BindToVariable("Foo W", "w");
            Editor.Binder.BindToVariable("Foo H", "h");
            //Editor.BindToType<Foo>();
            this.DoubleBuffered = true;
            
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush b = new SolidBrush(Color.Black);
            g.FillRectangle(b, MyFoo.GetRectangle());
            g.FillRectangle(b, MyFoo2.GetRectangle());
            g.FillRectangle(b, MyFoo3.GetRectangle());
            g.FillRectangle(b, MyFoo4.GetRectangle());
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            
        }
        bool b = true;
        private void button2_Click(object sender, EventArgs e)
        {
            var field_service = Editor.ServiceProvider.Request<FieldLocatorService>(new byte[1]{ 0x1 });
            var pages_service = Editor.ServiceProvider.Request<PageLocatorService>();

            var tem = field_service.LocateName("Foo 7");

            var field = field_service.LocatePredicate<string>(predict:(value, fs) =>
            {
                return "Foo C".Equals(fs.Name);
            });

            var a = pages_service.ToArray();
            Editor.Binder.BindToObject(MyFoo3, false);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }
    }
    public class Foo 
    {
        public int x = 360 - 25;
        public int y = 360 - 25;
        public int w = 50;
        public int h = 50;
        public int i = 1;
        public int j = 2;
        public int k = 3;
        public int u = 4;
        public Rectangle GetRectangle() { return new Rectangle(x, y, w, h); }
        
        public Foo(int a, int b, int c, int d)
        {
            x = a;
            y = b;
            w = c;
            h = d;
            

        }
    }
    
}
