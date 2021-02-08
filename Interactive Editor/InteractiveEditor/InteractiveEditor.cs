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

    
    public class InteractiveEditor : ITwoWayBinderReciever , IManagerService, IDisposable
    {
        
      
        #region Ctror and initialization methods

        private InteractiveEditor(Form ownerWin, Type _T, string name, int x, int y, int width, int height, ControlFlags flags = ControlFlags.None)
        {
            OwnerForm = ownerWin;
            T = _T;
            CFlags = flags;
            Name = name;
            Location = new Point(x, y);
            Size = new Size(width, height);
            Initialize();
        }

        public static InteractiveEditor GenerateMyEditor<T>(Form ownerWin, string name, int x, int y, int width, int height, ControlFlags flags = ControlFlags.None)
        {
            return new InteractiveEditor(ownerWin, typeof(T), name, x, y, width, height, flags);
        }

        public static InteractiveEditor GenerateMyEditor(Form ownerWin, string name, int x, int y, int width, int height, ControlFlags flags = ControlFlags.None)
        {
            return new InteractiveEditor(ownerWin, null, name, x, y, width, height, flags);
        }

        public static InteractiveEditor GeneratePage(InteractiveEditor Owner)
        {
            var newPage = new InteractiveEditor(
                Owner.OwnerForm,
                Owner.T,
                Owner.Name,
                Owner.Location.X,
                Owner.Location.Y,
                Owner.Size.Width,
                Owner.Size.Height,
                Owner.CFlags);
            Owner.OwnerForm.Controls.Add(newPage.BackPanel);
            return newPage;
        }

        private void Initialize()
        {

            ShowText = true;
            AutoSize = true;
            BackPanel.Location = Location;
            BackPanel.Size = Size;
            BackPanel.Text = Name;
            BackPanel.Visible = Visible;
            BackPanel.Enabled = true;
            AvailableFields = T?.GetFields();

            VisibleChanged += new EventHandler(this.OnVisibleChanged);
            PageUp += new EventHandler(this.OnPageUp);
            PageDown += new EventHandler(this.OnPageDown);
            PagesChanged += new EventHandler(this.OnPagesChanged);
            Clear += new EventHandler(this.OnClear);

            // MISC CONTROLS INIT

            // 
            // NEXT PAGE BUTTON
            //
            MiscControls.Add(MiscControl.NextPage, new Button()
            {
                Visible = CFlags.HasFlag(ControlFlags.EnablePages),
                Enabled = !(NextPage == null),
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
                Enabled = !(PrevPage == null),
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


        public IManipulatorService Manipulator => ServiceProvider.Request<ManipulatorService>();


        public IBindingService Binder => ServiceProvider.Request<BindingService>();


        public IInvokerService Invoker => ServiceProvider.Request<InvokerService>();

        #endregion
     
        
        
        #region Default values

        public static readonly int defaultFieldHeight = 30;
        public static readonly Padding defaultMargins = new Padding(5, 15, 15, 0);
        public static readonly int defaultHSpacing = 5;

        #endregion



        #region NonPublic or readonly properties

        public Type T { get; }
        public readonly Form OwnerForm;
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
        protected Control _BackPanel;

        protected ControlFlags _CFlags = ControlFlags.None;



        protected bool _Visible = true;
        protected bool _ShowText = true;
        protected bool _AutoSize = true;
        protected int _Horizontal_Spacing = defaultHSpacing;
        protected int _FieldHeight = defaultFieldHeight;
        protected Padding _Margins = defaultMargins;
        protected bool _AutoUpdateEnabled = true;

        protected InteractiveEditor _NextPage;
        protected InteractiveEditor _PrevPage;
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

        public InteractiveEditor NextPage
        {
            get => _NextPage;
            set
            {
                if (_NextPage != value)
                {
                    _NextPage = value;
                    PagesChanged?.Invoke(this, null);
                }
            }
        }


        public InteractiveEditor PrevPage
        {
            get => _PrevPage;
            set
            {
                if (_PrevPage != value)
                {
                    _PrevPage = value;
                    PagesChanged?.Invoke(this, null);
                }

            }
        }


        public Control BackPanel
        {
            get
            {
                return _BackPanel = _BackPanel ?? new GroupBox();
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


        #endregion



        #region Events

        public event EventHandler VisibleChanged;
        public event EventHandler PageUp;
        public event EventHandler PageDown;
        public event EventHandler PagesChanged;
        public event EventHandler Clear;



        #endregion



        #region Default Event Implementations

        public void OnFieldVisibilityChanged(object sender, FieldChangedEventArgs e)
        {
            Fieldset field = sender as Fieldset;
            var delta = field.Visible ? Horizontal_Spacing + FieldHeight : -Horizontal_Spacing + -FieldHeight;
            for (int i = e.index; i < Fields.Count; i++)
                Fields[i].BackPanel.Location = new Point(Fields[i].BackPanel.Location.X, Fields[i].BackPanel.Location.Y + delta);


        }



        private void OnVisibleChanged(object sender, EventArgs e = null)
        {
            BackPanel.SuspendLayout();
            BackPanel.Visible = Visible;
            BackPanel.ResumeLayout();
        }



        public void OnPageUp(object sender, EventArgs e)
        {
            NextPage.Visible = true;
            this.Visible = false;
        }



        public void OnPageDown(object sender, EventArgs e)
        {
            if (PrevPage == null)
            {
                Console.WriteLine("prev page is null");
            }
            else
            {
                PrevPage.Visible = true;
                this.Visible = false;
            }

        }



        private void OnPagesChanged(object sender, EventArgs e)
        {
            MiscControls[MiscControl.PrevPage].Enabled = !(PrevPage == null);
            MiscControls[MiscControl.NextPage].Enabled = !(NextPage == null);
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
            do
            {
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
                actualPage = actualPage.NextPage;
            } while (actualPage != null);
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
