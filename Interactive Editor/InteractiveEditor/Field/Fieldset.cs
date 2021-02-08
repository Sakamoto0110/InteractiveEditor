﻿using Editor.BindingInterface;
using Editor.Fields.Events;
using Editor.Misc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using static Editor.Modifiers;

namespace Editor.Fields
{
    public class Fieldset 
    {

        public readonly string Name = "default";
        public string LabelText = "";
        protected Point _AnchorPoint = new Point(0, 0);
        public Point AnchorPoint
        {
            get => _AnchorPoint;
            set
            {
                _AnchorPoint = value;
                if (BackPanel != null)
                    BackPanel.Location = _AnchorPoint;
            }
        }
        protected InteractiveEditor Owner;

        public readonly int Index = -1;
        public FieldFlags FFlags;
        //
        // Generic properties
        //
        protected bool _AutoUpdateEnabled = true;
        public bool AutoUpdateEnabled
        {
            get => _AutoUpdateEnabled;
            set => _AutoUpdateEnabled = value;
        }
        private bool _Visible = true;
        public bool Visible
        {
            get => _Visible;
            set
            {
                if (_Visible != value)
                {
                    _Visible = value;
                    BackPanel.Visible = _Visible;
                    FieldVisibleChanged?.Invoke(this, new FieldChangedEventArgs(this.Index));
                    
                }

            }
        }
        //
        // Controls
        //
        public Label QuestionMark;
        public Label Label;
        public Control Field;
        public Panel BackPanel;
        public List<Control> MiscControls;
        //
        // Binding properteies
        //            
        public FieldInfo FieldInfo;
        public string FieldName;
        public bool AllowBind = true;
        //
        // Unique bind properties
        //
        public object TargetInstance;
        public object FieldData;
        //
        // MultiBind properties
        //
        public List<object> TargetInstanceList = new List<object>();
        public List<FieldInfo> FieldInfoList = new List<FieldInfo>();
        public bool MultiBind = false;
        //
        // Cap functions
        //
        public CapFunction CapFunction;
        public object CapParms;
        public Func<object, object, object, object> TheBrute;
        //
        // Other
        //
        public double TrackBarMultiplier = 1;
        public double HSliderMultiplier = 1;
        private double _Delta;
        public double Delta
        {
            get => _Delta;
            set => _Delta = value;
        }


        //
        // Events
        //
        public event EventHandler<FieldChangedEventArgs> FieldVisibleChanged;
        public event EventHandler<FieldBinderEventArgs> FieldPreBind;
        public event EventHandler<FieldBinderEventArgs> FieldBindStarted;
        public event EventHandler<FieldBinderEventArgs> FieldBindFinished;
        public event EventHandler<FieldBinderFailureEventArgs> FieldBindFailure;

        public Fieldset(InteractiveEditor owner, int _index, string name, Type U, int _x, int _y, FieldFlags flags)
        {
            Index = _index;
            Owner = owner;
            Name = name;
            LabelText = Name;
            AnchorPoint = new Point(_x, _y);
            FFlags = flags;
            Initialize(U);
        }


        private void Initialize(Type type)
        {
            MiscControls = new List<Control>();

            //
            // BackPanel 
            //
            BackPanel = new Panel();
            BackPanel.Size = new Size(Owner.Size.Width - Owner.Margins.Right, 30);
            BackPanel.Location = new Point(_AnchorPoint.X + Owner.Margins.Left, _AnchorPoint.Y + Owner.Margins.Top);
            BackPanel.Parent = Owner.BackPanel;
            BackPanel.BringToFront();
            //
            // Label
            //
            Label = new Label();
            Label.Text = Name;
            Label.Parent = BackPanel;
            Label.Cursor = FFlags.HasFlag(FieldFlags.UseHandCursorLabel) ? Cursors.Hand : Cursors.Default;
            Label.Dock = DockStyle.Left;
            //
            // Question Mark
            //
            QuestionMark = new Label();
            QuestionMark.Text = "(?)";
            QuestionMark.Enabled = false;
            QuestionMark.Parent = BackPanel;
            QuestionMark.Size = new Size(20, 10);
            QuestionMark.Dock = DockStyle.Left;
            //
            // Field Unique properties ( TextBox, ComboBox or TrackBar properties )
            //
            if (type == typeof(TextBox)) TextBoxInit();
            if (type == typeof(ComboBox)) ComboBoxInit();
            if (type == typeof(TrackBar)) TrackBarInit();
            if (type == typeof(Button)) ButtonInit();
            //
            // Field Generic properties ( Control properties )
            //                             
            //Field.Dock = DockStyle.Right;
            Field.Width = 100;
            Field.Location = new Point(BackPanel.Width - Field.Width, 0);
            Field.Parent = BackPanel;

            Field.Enabled = !FFlags.HasFlag(FieldFlags.IsNotEnabled);
            Field.BringToFront();
            // 
            // Post init
            //
            if (FFlags.HasFlag(FieldFlags.UseHSliderControl))
            {
                Label.Cursor = Cursors.SizeWE;
                Label.MouseDown += new MouseEventHandler((s, e) => HSlider_MouseDown(s, e));
                Label.MouseMove += new MouseEventHandler((s, e) => HSlider_MouseMove(s, e, this));
                Label.MouseUp += new MouseEventHandler((s, e) => HSlider_MouseUp(s, e));
            }
            FieldVisibleChanged += new EventHandler<FieldChangedEventArgs>((s,e)=>Owner.OnFieldVisibilityChanged(s,e));

            void ButtonInit()
            {
                Field = new Button();

            }
            void TextBoxInit()
            {
                Field = new TextBox();
                Field.Cursor = FFlags.HasFlag(FieldFlags.UseHandCursorField) ? Cursors.Hand : Cursors.IBeam;
                ((TextBox)Field).ReadOnly = FFlags.HasFlag(FieldFlags.IsReadOnly);
            }
            void ComboBoxInit()
            {
                Field = new ComboBox();
                Field.Cursor = FFlags.HasFlag(FieldFlags.UseHandCursorField) ? Cursors.Hand : Cursors.Default;
            }
            void TrackBarInit()
            {
                Field = new TrackBar();
                Field.Cursor = FFlags.HasFlag(FieldFlags.UseHandCursorField) ? Cursors.Hand : Cursors.Default;
            }
        }



        public void Destroy()
        {
            BackPanel.Controls.Remove(Field);
            BackPanel.Controls.Remove(Label);
            BackPanel.Controls.Remove(QuestionMark);
            Owner.BackPanel.Controls.Remove(BackPanel);
        }
        private InteractiveEditor GetOwner()
        {
            var actualPage = Owner;
            while (actualPage.PrevPage != null)
                actualPage = actualPage.PrevPage;
            return actualPage;
        }
        public void Clear()
        {
            Unbind();

            switch (Field)
            {
                case TextBox textBox:
                    textBox.Text = " ";
                    textBox.BackColor = Control.DefaultBackColor;
                    textBox.ForeColor = Control.DefaultForeColor;
                    break;
                case ComboBox comboBox:
                    comboBox.SelectedIndex = -1;
                    break;
                case TrackBar trackBar:
                    trackBar.Value = (trackBar.Maximum - trackBar.Minimum) / 2;
                    break;
            }
        }
        private void OnFailure(FieldBinderFailureEventArgs args, [CallerLineNumber] int line = 0, [CallerMemberName] string callerName = null)
        {
            //AllowBind = false;
            args.Line = line;
            args.Source = callerName;
            var rootEditor = GetOwner();
            FieldBindFailure?.Invoke(this, args);
        }

        public void BindToField(Type T, string varName, CapFunction f = null, object capParms = null)
        {
            CapParms = capParms;
            CapFunction = f;
            FieldInfo = T.GetField(varName);
            FieldName = varName;

            var FieldBinderArgs = new FieldBinderEventArgs();
            FieldBinderArgs.FieldInfo = FieldInfo;
            FieldBinderArgs.TargetFieldName = varName;
            FieldBinderArgs.VariableFieldName = Name;

            FieldPreBind?.Invoke(this, FieldBinderArgs);
            if (FieldInfo == null)
            {


                OnFailure(new FieldBinderFailureEventArgs()
                {
                    Message = $"Unable to get FieldInfo data from variable: {varName} in Type: {T}",
                    Reasson = $"FieldInfo returned null",
                    PossibleSolution = $"Check variable name in declaration",
                    TargetFieldName = varName,
                    VariableFieldName = Name,
                });


            }
        }
        public void BindToObject(object instance, bool multiBind, Func<object, object, object, object> bruteForce)
        {
            if (!AllowBind)
                return;
            if (!multiBind || TargetInstanceList.Count == 0)
                Unbind();

            MultiBind = multiBind;
            TheBrute = bruteForce;
            TargetInstance = instance;
            if (FieldName == null)
            {
                OnFailure(new FieldBinderFailureEventArgs()
                {
                    Message = $"Unable to bind to object {instance}",
                    Reasson = $"FieldName returned null",
                    PossibleSolution = $"Check variable name in declaration",
                    VariableFieldName = Name,
                });
                return;
            }
            if (TargetInstance is IVarProvider varProvider)
            {
                if (varProvider.VariablePool[0].ToLower().Equals("blacklist"))
                    for (int i = 1; i < varProvider.VariablePool.Length; i++)
                        if (varProvider.VariablePool[i].Equals(FieldName))
                        {
                            Logger.DoLog(this, $"{FieldName} is a blacklisted field, aborting binding and disabling field!", 1);
                            this.Visible = false;
                            return;
                        }
                        else
                        {
                            this.Visible = true;
                        }
                if (varProvider.VariablePool[0].ToLower().Equals("partial blacklist"))
                    for (int i = 1; i < varProvider.VariablePool.Length; i++)
                        if (varProvider.VariablePool[i].Equals(FieldName))
                        {
                            Logger.DoLog(this, $"{FieldName} is a partial blacklisted field, aborting binding!", 1);
                            return;

                        }
            }



            FieldInfo = instance.GetType().GetField(FieldName, BindingFlags.Public | BindingFlags.Instance);
            FieldData = FieldInfo.GetValue(TargetInstance);

            var FieldBinderArgs = new FieldBinderEventArgs();
            FieldBinderArgs.FieldInfo = FieldInfo;
            FieldBinderArgs.TargetFieldName = FieldName;
            FieldBinderArgs.VariableFieldName = Name;
            FieldBinderArgs.FieldData = FieldData;
            FieldBinderArgs.TargetInstance = TargetInstance;
            FieldBindStarted?.Invoke(this, FieldBinderArgs);


            if (multiBind || TargetInstanceList.Count == 0)
            {
                FieldInfoList.Add(FieldInfo);
                TargetInstanceList.Add(instance);
                if (TargetInstanceList.Count > 1)
                    return;
            }
            if (FieldData == null)
            {
                OnFailure(new FieldBinderFailureEventArgs()
                {
                    Message = $"Unable to bind to object {instance}",
                    Reasson = $"FieldData returned null",
                    PossibleSolution = $"Check variable name in declaration",
                    VariableFieldName = Name,
                });
                return;
            }
            if (instance is ITwoWayBinderTransmiter twoWayBinder)
            {
                twoWayBinder.BindedTo = this.GetOwner();
            }
            Type dT = FieldData.GetType();
            Field.BackColor = dT == typeof(Color) ? Color.FromArgb(255, (Color)FieldData) : Control.DefaultBackColor;
            Field.ForeColor = dT == typeof(Color) ? Color.FromArgb(255, (Color)FieldData) : Control.DefaultForeColor;
            switch (Field)
            {
                case TextBox textBox:
                    textBox.Text = $"{FieldData}";
                    textBox.TextChanged += ValueChanged;
                    break;
                case ComboBox comboBox:
                    comboBox.SelectedItem = FieldData;
                    comboBox.SelectedIndexChanged += ValueChanged;
                    break;
                case TrackBar trackBar:
                    trackBar.Value = (int)Convert.ToInt32(Convert.ToDouble(FieldData) * TrackBarMultiplier);
                    trackBar.ValueChanged += ValueChanged;
                    break;
            }
            FieldBindFinished?.Invoke(this, FieldBinderArgs);
        }

        public void Invalidate()
        {

        }

        public void Unbind()
        {
            if (TargetInstance is ITwoWayBinderTransmiter twoWayBinder)
            {
                twoWayBinder.BindedTo = null;
            }
            AllowBind = true;
            FieldInfoList.Clear();
            TargetInstanceList.Clear();

            switch (Field)
            {
                case TextBox textBox:

                    textBox.TextChanged -= ValueChanged;
                    break;
                case ComboBox comboBox:
                    comboBox.SelectedIndexChanged -= ValueChanged;
                    break;
                case TrackBar trackBar:
                    trackBar.ValueChanged -= ValueChanged;
                    break;
            }
            TargetInstance = null;
            FieldData = null;
            FieldInfo = null;
        }
        public void ValueChanged(object sender, EventArgs e)
        {
            if (!_AutoUpdateEnabled)
            {
                return;
            }
            if (MultiBind)
            {
                for (int i = 0; i < TargetInstanceList.Count; i++)
                {
                    object instance = TargetInstanceList[i];
                    object data = FieldInfoList[i].GetValue(instance);
                    if (data.GetType() == typeof(Int32) || data.GetType() == typeof(double))
                    {
                        double oldVal = Convert.ToDouble(data);
                        var newVal = oldVal + _Delta;
                        switch (data)
                        {
                            case int _:
                                newVal = Convert.ToInt32(newVal);
                                FieldInfoList[i].SetValue(instance, (int)newVal);
                                break;
                            case double _:
                                FieldInfoList[i].SetValue(instance, newVal);
                                break;

                        }
                    }
                    else
                    {
                        // Other data types like enums 

                        switch (sender)
                        {
                            case TextBox textbox:

                                if (data.GetType() == typeof(Color))
                                    data = TextBoxColorBinder(textbox);
                                else
                                    data = TextBoxBinder(textbox);
                                break;
                            case TrackBar trackBar:
                                data = TrackBarBinder(trackBar);
                                break;
                            case ComboBox comboBox:
                                data = ComboBoxBinder(comboBox);
                                break;
                        }

                        try
                        {
                            FieldInfoList[i].SetValue(instance, data);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Unable to apply multi bind");
                            Console.WriteLine(ex);
                        }
                    }




                }
                return;
            }



            switch (sender)
            {
                case TextBox textbox:

                    if (FieldData.GetType() == typeof(Color))
                        FieldData = TextBoxColorBinder(textbox);
                    else
                        FieldData = TextBoxBinder(textbox);
                    break;
                case TrackBar trackBar:
                    FieldData = TrackBarBinder(trackBar);
                    break;
                case ComboBox comboBox:
                    FieldData = ComboBoxBinder(comboBox);
                    break;
            }



            // Binding problems? Just invoke The Brute :D
            //FieldData = TheBrute == null ? FieldData : TheBrute?.Invoke(FieldData,this,sender);
            FieldData = TheBrute?.Invoke(FieldData, sender, this);

            // Pass value of the textbox to the variable;
            try
            {
                FieldInfo.SetValue(TargetInstance, FieldData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            object TextBoxBinder(TextBox textbox)
            {

                // Cap the textbox value
                if (CapFunction != null)
                {
                    Delegate[] fList = CapFunction?.GetInvocationList();

                    if (fList?.Length > 1)
                    {
                        for (int i = 1; i < fList.Length; i++)
                        {
                            CapFunction MulticastDelegate;
                            MulticastDelegate = (CapFunction)fList[i];
                            textbox.Text = (string)MulticastDelegate.Invoke(((CapFunction)fList[i - 1]).Invoke(textbox.Text, CapParms), CapParms);
                        }
                    }
                    else
                    {
                        textbox.Text = (string)CapFunction.Invoke(textbox.Text, CapParms);
                    }

                    textbox.Select(textbox.Text.Length, 0);
                }


                // Convert the value of textbox to the value of the target variable
                try
                {
                    switch (FieldData)
                    {
                        case int _:
                            FieldData = Convert.ToInt32(textbox.Text);
                            break;
                        case double _:
                            FieldData = Convert.ToDouble(textbox.Text);
                            break;
                        default:
                            FieldData = textbox.Text;
                            break;
                    }
                }
                catch (Exception ex)
                {
                   // DoLog(this, ex.Message, 2);
                };


                return FieldData;

            }
            object TextBoxColorBinder(TextBox textbox)
            {
                return textbox.BackColor;

            }
            object TrackBarBinder(TrackBar trackBar)
            {
                return trackBar.Value;
            }
            object ComboBoxBinder(ComboBox comboBox)
            {

                return comboBox.SelectedValue;
            }




        }


        private static void HSlider_MouseDown(object sender, MouseEventArgs e)
        {
            Label l = (Label)sender;
            l.Tag = new Point(e.X, e.Y);

        }
        private static void HSlider_MouseMove(object sender, MouseEventArgs e, Fieldset fs)
        {
            if (fs.TargetInstance == null)
                return;
            Label l = (Label)sender;
            if (l.Tag != null)
            {
                Point m = (Point)l.Tag;
                Point delta = new Point(e.X - m.X, e.Y - m.Y);
                l.Tag = new Point(e.X, e.Y);
                if (fs.Field.GetType() == typeof(TextBox))
                {
                    TextBox tb = (TextBox)fs.Field;
                    double oldV = Convert.ToDouble(tb.Text);
                    double newVal;
                    if (fs.CapFunction != null)
                    {
                        string fixText = $"{fs.CapFunction?.Invoke((delta.X * fs.HSliderMultiplier) + oldV, fs.CapParms)}";
                        newVal = (double)Convert.ToDouble(fixText);
                    }
                    else
                    {
                        newVal = (delta.X * fs.HSliderMultiplier) + oldV;
                    }
                    fs.Delta = delta.X;
                    tb.Text = $"{newVal:0.###}";
                }

            }
        }
        private static void HSlider_MouseUp(object sender, MouseEventArgs e)
        {
            Label l = (Label)sender;
            if (l.Tag != null)
                l.Tag = null;
        }
    }
}