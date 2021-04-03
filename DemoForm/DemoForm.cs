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
using Editor.Controller;
using Editor.Fields;
using Editor.Misc;
using Editor.Options;
using Editor.Services;
using Editor.Services.BinderService.Mapping;
using MyEditorControl.TestObjects;
using static Editor.Modifiers;
using static Editor.Services.BindingService;

namespace MyEditorControl
{
    public partial class DemoForm : Form
    {
        public InspectorController Inspector;
        public List<string> CommandHistory = new List<string>();
        public int CommandIndex = 0;

        public Foo MyFoo = new Foo(300, 50, 50, 50);

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
        bool b = true;
        public void SelectFoo0()
        {
            
            
           
            Console.WriteLine("SelectFoo0 called");
            
        }
        void init()
        {
            textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBox1.AutoCompleteCustomSource = new AutoCompleteStringCollection();
            textBox1.AutoCompleteCustomSource.AddRange(ComList);
           // this.Size = new Size(480,480);
            this.Paint += Form1_Paint;
            this.DoubleBuffered = true;
        }
        public DemoForm()
        {
            Editor.Options.GlobalOptions.RootRequireAttrTypeSafeLock = true;
            InspectorOptions options = new InspectorOptions()
            {
                CreateOwnWindow = true,
                DockStyle = DockStyle.Fill,
                Name = "Foo",
                Location = new Point(10,50),
                Size = new Size(250,400),
                CanCollapse = true,                
            };
            
            









            
            Inspector = InspectorController.BuildInspector<Foo>(options,
                new BindingConfigurator((map) =>
                {
                    map.Modify("x", (ref BindingArgs args) => 
                    {
                        args.FieldFlags = FieldFlags.UseHSliderControl;
                        args.capFunction = ONLY_NUMBERS + POSITIVE_NUMBERS;
                        args.capParms = new[] { "3" };
                        args.Post = (fs) =>
                        {
                            fs.LabelText = "Renamed X";
                        };
                    });
                }));
            












            InitializeComponent();
            init();
            Init_EXECUTE_T0();

            
            
            

            
            Inspector.AttatchToWindow(this);

            //Inspector.Modify.BuildFieldsForTypeByMapping(Mapping.CreateTypoToInspectorFieldMapping<Foo>(null));

            Inspector.Invoker.InvokeForAllFields(
                (fs) =>
                {
                    if(fs.GroupSize > 0)
                    {
                        //Console.WriteLine(fs.Name + " " + fs.Children.Count);
                    }
                    else
                    {
                        //Console.WriteLine(fs.Name + " " + fs.GroupFieldInfo.Count);
                    }
                    
                });
            

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

        private void button1_Click(object sender, EventArgs e)
        {
            Inspector.Binder.BindToObject(MyFoo);
        }
    }
   
    
    
}

