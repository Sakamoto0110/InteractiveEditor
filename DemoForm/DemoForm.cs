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
using Editor.Misc;
using Editor.Services;
using MyEditorControl.TestObjects;
using static Editor.Services.BindingService;

namespace MyEditorControl
{
    public partial class DemoForm : Form
    {
        public InteractiveEditor Editor;
        public List<string> CommandHistory = new List<string>();
        public int CommandIndex = 0;

        public Foo MyFoo = new Foo(380, 50, 50, 50);

        public static readonly string[] ComList = new string[]
        {
            "select",
            "multiselect"
        };
        public Dictionary<string, Action> EXECUTE_T0;
       
        public void Init_EXECUTE_T0()
        {
            EXECUTE_T0 = new Dictionary<string, Action>()
            {
                {"select", SelectFoo0 }
            };
        }
        public void SelectFoo0()
        {
            Console.WriteLine("SelectFoo0 called");
            Editor.Binder.BindToObject(MyFoo);
        }
        void init()
        {
            textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBox1.AutoCompleteCustomSource = new AutoCompleteStringCollection();
            textBox1.AutoCompleteCustomSource.AddRange(ComList);
            this.Size = new Size(480,480);
            this.Paint += Form1_Paint;
            this.DoubleBuffered = true;
        }
        public DemoForm()
        {
            InitializeComponent();
            init();
            Init_EXECUTE_T0();

            Editor = InteractiveEditor.GenerateMyEditor<Foo>(this, "Foo", 10, 10, 250, 400);

            //Editor.Binder.BindToTypo(new string[] { "x", "y", "width" }, FilterMode.Whitelist) ;
            
            //Editor.Binder.BindToTypo();
            Editor.Horizontal_Spacing = 0;
            Editor.FieldHeight = 23;
            Editor.Binder.BindToTypo(new string[] { "x", "width" }, FilterMode.Blacklist);
            //Editor.Binder.BindToTypo(new BindingConfigurator(
            //    (ref Dictionary<string, PreBindingArgs> MappedVariable) =>
            //    {
            //        MappedVariable["x"] = new PreBindingArgs(MappedVariable["x"])
            //        {
            //            FieldSet_FieldType = typeof(Separator),
            //        };

            //    }));




            Controls.Add(Editor.BackPanel);

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            MyFoo.Render(g);
        }

      
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

     
        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            var tb = sender as TextBox;
            if (e.KeyCode == Keys.Enter)
            {
                var command = $"{textBox1.Text}";
                if(command.Split(' ').Length == 1)
                {
                    EXECUTE_T0[command].Invoke();
                }
                else
                {

                }
                CommandHistory.Add(textBox1.Text);
                CommandIndex = CommandHistory.Count;
                textBox1.Clear();
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (CommandHistory.Count >= 1)
                {
                    if (CommandIndex > 0)
                        CommandIndex--;
                    textBox1.Text = CommandHistory[CommandIndex];
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (CommandHistory.Count >= 1)
                {
                    if (CommandIndex < CommandHistory.Count - 1)
                        CommandIndex++;
                    textBox1.Text = CommandHistory[CommandIndex];
                }
            }
            else if (e.KeyCode == Keys.Tab)
            {
                textBox1.Select(0, 0);
            }
        }
    }
   
    
    
}

