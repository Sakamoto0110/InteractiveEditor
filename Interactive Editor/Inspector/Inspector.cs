using Editor.Services;
using Editor.Fields;
using Editor.Fields.Events;
using Editor.Misc;
using Editor.BindingInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using static Editor.Modifiers;

namespace Editor
{
    // ways to use:
    // 1: manual -> manually: create each field, bind field, bind to object
    
    // 3: automatic -> manually: bind to typo, bind to object. Auto: field creation
    public enum TypeBinderMode
    {
        Automatic = 1,
        Manual    = 2,
    }
    public class Configurator
    {
        public ControlFlags controlFlags;
        public TypeBinderMode typeBinderMode;

        public Configurator()
        {
            controlFlags = ControlFlags.EnableClear  |
                           ControlFlags.EnableApply  |
                           ControlFlags.EnablePages  |
                           ControlFlags.EnableReload |
                           ControlFlags.EnableQuestionMark;
            typeBinderMode = TypeBinderMode.Manual;
        }
    }
    
    public class Inspector : ITwoWayBinderReciever , IManagerService, IDisposable
    {


        #region Ctror and initialization methods
        
        private Inspector(Type _T, string name, Point location, Size size, Configurator _configurator = null)
        {
            T = _T;
            Name = name;
            Location = new Point(location.X,location.Y);
            Size = new Size(size.Width, size.Height);
            Config = _configurator;
            Initialize();
        }

        public static Inspector Build<T>(string name, Point location, Size size, Configurator configurator = null)
        {
            return new Inspector(typeof(T), name, location, size, configurator);
        }

        public static Inspector Build(string name, Point location, Size size, Configurator configurator = null)
        {
            return new Inspector(null, name, location, size, configurator);
        }

       

        private void Initialize()
        {

            ShowText = true;
            AutoSize = true;
            BackPanel.Location = Location;
            BackPanel.Size = Size;
            BackPanel.Text = Name;
            BackPanel.Font = new Font(BackPanel.Font, FontStyle.Bold);
            BackPanel.Visible = true;
            BackPanel.Enabled = true;
            AvailableFields = T?.GetFields();

            VisibleChanged += new EventHandler(this.OnVisibleChanged);
            Clear += new EventHandler(this.OnClear);
            MouseWheel += new MouseEventHandler(this.OnMouseWheel);
            Scroll += new ScrollEventHandler(this.OnScroll);

            BackPanel.MouseWheel += MouseWheel;

            CFlags = Config.controlFlags;
            TypeBinderMode = Config.typeBinderMode;


            // MISC CONTROLS INIT

            // 
            // NEXT PAGE BUTTON
            //
            MiscControls.Add(MiscControl.NextPage, new Button()
            {
                Visible = CFlags.HasFlag(ControlFlags.EnablePages),
                Enabled = false,
                Text = "\u25B6",
                Name = "Next_Page",
                Size = new Size(30, 30),
                Location = new Point(BackPanel.Width - 35, BackPanel.Height - 35),
                Parent = BackPanel,
            });
            //
            // PREV PAGE BUTTON
            //
            MiscControls.Add(MiscControl.PrevPage, new Button()
            {
                Visible = CFlags.HasFlag(ControlFlags.EnablePages),
                Enabled = false,
                Text = "\u25C0",
                Name = "Prev_Page",
                Size = new Size(30, 30),
                Location = new Point(5, BackPanel.Height - 35),
                Parent = BackPanel,
            });
            //
            // RELOAD BUTTON
            //
            MiscControls.Add(MiscControl.Reload, new Button()
            {
                Visible = CFlags.HasFlag(ControlFlags.EnableReload),
                Enabled = CFlags.HasFlag(ControlFlags.EnableReload),
                BackgroundImageLayout = ImageLayout.Zoom,
                Text = "R",
                Name = "Reload",
                Size = new Size(30, 30),
                Location = new Point(BackPanel.Width / 2 - 15 - 30, BackPanel.Height - 35),
                Parent = BackPanel,
            });
            //
            // APPLY BUTTON
            //
            MiscControls.Add(MiscControl.Apply, new Button()
            {
                Visible = CFlags.HasFlag(ControlFlags.EnableApply),
                Enabled = CFlags.HasFlag(ControlFlags.EnableApply),
                BackgroundImageLayout = ImageLayout.Zoom,
                Text = "A",
                Name = "Apply",
                Size = new Size(30, 30),
                Location = new Point(BackPanel.Width / 2 - 15 + 30, BackPanel.Height - 35),
                Parent = BackPanel,
            });
            //
            // UNBIND BUTTON
            //
            MiscControls.Add(MiscControl.Clear, new Button()
            {
                Visible = CFlags.HasFlag(ControlFlags.EnableClear),
                Enabled = CFlags.HasFlag(ControlFlags.EnableClear),
                BackgroundImageLayout = ImageLayout.Zoom,
                Text = "U",
                Name = "Unbind",
                Size = new Size(30, 30),
                Location = new Point(BackPanel.Width / 2 - 15, BackPanel.Height - 35),
                Parent = BackPanel,
            });
            MiscControls[MiscControl.Clear].Click += (s, e) => Clear?.Invoke(s, e);
            Logger.DoLog(this, "MyEditor sucessfully initialized!");

        }

       


        #endregion



        

        #region Services interfaces 

        public _IServiceProvider ServiceProvider => new IOBServiceProvider(this);


        public IFieldLocatorService LocateField => ServiceProvider.Request<FieldLocatorService>();


 
        public IManipulatorService Modify => ServiceProvider.Request<ManipulatorService>();


        public IBindingService Binder => ServiceProvider.Request<BindingService>();


        public IInvokerService Invoker => ServiceProvider.Request<InvokerService>();

        #endregion
     
        
        
        #region Default values

        public static readonly int defaultFieldHeight = 30;
        public static readonly int defaultHSpacing = 5;
        public static readonly Padding defaultMargins = new Padding(5, 15, 15, 0);
        public static readonly int defaultMiscControlHeight = 35;

        public static readonly ControlFlags defaultControlFlags = ControlFlags.EnableClear  | 
                                                                  ControlFlags.EnableApply  | 
                                                                  ControlFlags.EnablePages  | 
                                                                  ControlFlags.EnableReload | 
                                                                  ControlFlags.EnableQuestionMark;
        public static readonly TypeBinderMode defaultTypeBinderMode = TypeBinderMode.Manual;

        #endregion



        #region NonPublic or readonly properties

        public Type T { get; }
        public Form OwnerForm;
        public readonly Dictionary<MiscControl, Control> MiscControls = new Dictionary<MiscControl, Control>();
        public FieldInfo[] AvailableFields;

        #endregion
       

             
        #region Properties declaration

        /// <summary>
        /// This properties will be available by interface
        /// </summary>
        protected string _Name = "default";
        protected Point _Location;
        protected Size _Size = new Size(100, 100);
        protected BackpanelHelper _BackPanel;

        protected Configurator _Config;
        
        protected ControlFlags _CFlags = ControlFlags.None;

        protected TypeBinderMode _TypeBiderMode = TypeBinderMode.Manual;

        protected int _MiscControlHeight = defaultMiscControlHeight;

        protected bool _Visible = true;
        protected bool _ShowText = true;
        protected bool _AutoSize = true;
        protected int _Horizontal_Spacing = defaultHSpacing;
        protected int _FieldHeight = defaultFieldHeight;
        protected Padding _Margins = defaultMargins;
        protected bool _AutoUpdateEnabled = true;

        protected Inspector _NextPage;
        protected Inspector _PrevPage;
        protected List<Fieldset> _Fields = new List<Fieldset>();

        #endregion



        #region Properties implementation

        public List<Fieldset> Fields
        {
            get => _Fields;
            //set => _Fields = value;
        }

        public ControlFlags CFlags
        {
            get => _CFlags;
            set => _CFlags = value;
        }
        public TypeBinderMode TypeBinderMode
        {
            get => _TypeBiderMode;
            set => _TypeBiderMode = value;
        }
        public Configurator Config
        {
            get
            {
                if(_Config == null)
                {
                    _Config = new Configurator()
                    {
                       
                    };
                }
                return _Config;
            }
            set => _Config = value;
            
        }
        


        public BackpanelHelper BackPanel
        {
            get
            {
                return _BackPanel = _BackPanel ?? new BackpanelHelper();
            }
        }


        public bool ShowText
        {
            get => _ShowText;
            set
            {
                _ShowText = value;
                BackPanel.Text = _ShowText ? Name : "";
            }
        }


        public bool AutoSize
        {
            get;
            set;
        }


        public int Horizontal_Spacing
        {
            get => _Horizontal_Spacing;
            set => _Horizontal_Spacing = value;
        }


        public int FieldHeight
        {
            get => _FieldHeight;
            set
            {
                _FieldHeight = value;
            }
        }


        public Padding Margins
        {
            get => _Margins;
            set
            {
                _Margins = value;
            }
        }


        public bool Visible
        {
            get => _Visible;
            set
            {
                if (_Visible != value)
                {
                    _Visible = value;
                    BackPanel.BringToFront();
                    VisibleChanged?.Invoke(this, null);

                }

            }
        }


        public bool AutoUpdateEnabled
        {
            get => _AutoUpdateEnabled;
            set
            {
                _AutoUpdateEnabled = value;
                Invoker.InvokeForAllFields((f) => f.AutoUpdateEnabled = _AutoUpdateEnabled);
            }
        }


        public string Name
        {
            get => _Name;
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    if (BackPanel != null)
                        BackPanel.Text = Name;
                }
            }
        }


        public Point Location
        {
            get => _Location;
            set
            {
                _Location = value;
                if (BackPanel != null)
                    BackPanel.Location = _Location;
            }
        }


        public Size Size
        {
            get => _Size;
            set => _Size = value;
        }

        public int MiscControlHeight
        {
            get => _MiscControlHeight;
            set => _MiscControlHeight = value;
        }
        


        #endregion



        #region Events

        public event EventHandler VisibleChanged;        
        public event EventHandler Clear;
        public event ScrollEventHandler Scroll;
        public event MouseEventHandler MouseWheel;

        #endregion



        #region Default Event Implementations



        public void OnFieldVisibilityChanged(object sender, FieldChangedEventArgs e)
        {
            Fieldset field = sender as Fieldset;


            if (field.GroupSize > 0)
            {
                for (int i = 0; i < field.GroupSize; i++)
                {
                    string targetName = field.GroupMembersName[i];
                    Fieldset f = LocateField.LocateName(targetName);
                    f.Visible = field.Visible && !field.IsCollapsed;

                }
            }

            var delta = field.Visible ? Horizontal_Spacing + FieldHeight : -Horizontal_Spacing + -FieldHeight;
            if (delta > 0)            
                for (int i = e.index; i < Fields.Count; i++)                
                    Fields[i].BackPanel.Location = new Point(Fields[i].BackPanel.Location.X, Fields[i].BackPanel.Location.Y + delta);                            
            else            
                for (int i = Fields.Count - 1; i >= e.index; i--)                
                    Fields[i].BackPanel.Location = new Point(Fields[i].BackPanel.Location.X, Fields[i].BackPanel.Location.Y + delta);                
            
            






        }



        private void OnVisibleChanged(object sender, EventArgs e = null)
        {
            BackPanel.SuspendLayout();
            BackPanel.Visible = Visible;
            BackPanel.ResumeLayout();
        }


        private void OnScroll(object sender, ScrollEventArgs e)
        {
            
        }
        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            int delta = 10 * (e.Delta > 0 ? 1 : -1);
            if(delta < 0)
            {
                for(int i = 0; i < Fields.Count; i++)
                {
                    var field = Fields[i];
                    var oldVal = new Point(field.BackPanel.Location.X, field.BackPanel.Location.Y);
                    field.BackPanel.Location = new Point(oldVal.X, oldVal.Y + delta);
                }
            }
            else
            {
                for (int i = Fields.Count-1; i >= 0; i--)
                {
                    var field = Fields[i];
                    var oldVal = new Point(field.BackPanel.Location.X, field.BackPanel.Location.Y);
                    field.BackPanel.Location = new Point(oldVal.X, oldVal.Y + delta);
                }
            }
            
        }
       
        

        private void OnClear(object sender, EventArgs e)
        {
            Invoker.InvokeForAllFields((f) => f.Clear());
        }

        #endregion



        #region Manual Event Triggers

        public void OnClear()
        {
            OnClear(null, null);
        }

        #endregion



        #region ITwoWayBinderReciever

        public void EditFieldValueByVariableName(string key, object value)
        {
            var actualPage = this;
            for (int i = 0; i < actualPage.Fields.Count; i++)
            {
                if (actualPage.Fields[i].FieldName.Equals(key))
                {
                    var Field = actualPage.Fields[i].Field;
                    var str = "";
                    if (value is int v)
                    {
                        str = $"{v}";
                    }
                    else if (value is float f)
                    {
                        str = $"{f}";
                    }
                    else if (value is double d)
                    {
                        str = $"{d}";
                    }
                    ((TextBox)Field).Text = str;
                    return;
                }
            }
        }

        #endregion



        #region Dispose
        public void Dispose()
        {
            for (int i = 0; i < Fields.Count; i++)
            {
                Fields[i].Destroy();
                foreach (Control c in MiscControls.Values)
                    BackPanel.Controls.Remove(c);
                OwnerForm.Controls.Remove(BackPanel);
                BackPanel.Dispose();
            }
            Fields.Clear();
        }

        #endregion


    }

}
