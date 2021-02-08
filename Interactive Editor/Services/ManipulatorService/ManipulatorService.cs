using Editor.Fields;
using Editor.Services;
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
    public class ManipulatorService : IManipulatorService, _IService
    {
        public IOBServiceProvider Provider { get; set; }
        public InteractiveEditor Owner => Provider.Owner;


        private FieldLocatorService FieldLocator => Provider.Request<FieldLocatorService>();
        private PageLocatorService PageLocator => Provider.Request<PageLocatorService>();

        #region Field

       
        public void AddField<U>(string fieldName, FieldFlags flags = FieldFlags.None)
        {
            if(FieldLocator.LocateName(fieldName) is null)
            {                
                if (Owner.CFlags.HasFlag(ControlFlags.EnablePages))
                    AddField_PagesFlagEnabled();
                else
                    AddField_PagesFlagDisabled();
            }
            
            void AddField_PagesFlagEnabled()
            {
                RECALC:
                var actualPage = PageLocator.LocateLast(); 
                
                var newY = (actualPage.Horizontal_Spacing * actualPage.Fields.Count) + (actualPage.Fields.Count * actualPage.FieldHeight);                
                // Predict if the the bounds of the new field will overlap with
                // the controls. If so, create a new page
                if (newY + actualPage.FieldHeight > actualPage.Size.Height - 35)
                {
                    actualPage.Modify.AddPage();
                    goto RECALC;                     
                }                
                actualPage.Fields.Add(new Fieldset(
                                          actualPage,
                                          actualPage.Fields.Count + 1,
                                          fieldName,
                                          typeof(U),
                                          0,
                                          newY,
                                          flags));
            }
           
            void AddField_PagesFlagDisabled()
            {
                Owner.Fields.Add(new Fieldset(
                               Owner,
                               Owner.Fields.Count + 1,
                               fieldName,
                               typeof(U),
                               0,
                               (Owner.Horizontal_Spacing * Owner.Fields.Count) + (Owner.Fields.Count * Owner.FieldHeight),
                               flags));
            }
        }


        public Control EditField(string key = null, bool GetPanel = false)
        {
            var target = key is null ? FieldLocator.LocateLast() : FieldLocator.LocateName(key);
            return target?.Field;
        }


        public Label EditFieldLabel(string key = null)
        {            
            return (key is null ? FieldLocator.LocateLast() : FieldLocator.LocateName(key))?.Label;
        }


        public void SetDeltaMultiplier(string key = null, double value = 1)
        {
            var target = key is null ? FieldLocator.LocateLast() : FieldLocator.LocateName(key);
            if (target != null)
                target.Delta = value;
        }


        public void SetHSliderMultiplier(string key = null, double value = 1)
        {
            var target = key is null ? FieldLocator.LocateLast() : FieldLocator.LocateName(key);
            if (target != null)
                target.HSliderMultiplier = value;
        }


        public void ToggleFieldVisible(string key, bool newVal)
        {
            var target = key is null ? FieldLocator.LocateLast() : FieldLocator.LocateName(key);
            if (target != null)
                target.Visible = newVal;
        }


        public void EditFieldValueByVariableName(string key, object value)
        {
            var target = FieldLocator.LocatePredicate(key, (v, t) => v.Equals(t.FieldName ?? ""));
            var str = "";
            if (value is int i)
            {
                str = $"{i}";
            }
            else if (value is float f)
            {
                str = $"{f}";
            }
            else if (value is double d)
            {
                str = $"{d}";
            }
            ((TextBox)target.Field).Text = str;

        }


        public object GetFieldValue<O>(string key)
        {
            var fs = FieldLocator.LocateName(key);
            if (fs is null)
                return null;
            var target = fs.Field;

            if (target is TextBox textBox)
            {
                if (typeof(O) == typeof(Int32) || typeof(O) == typeof(double))
                {
                    if (typeof(O) == typeof(Int32)) return Convert.ToInt32(textBox.Text);
                    return Convert.ToDouble(textBox.Text);
                }
                if (typeof(O) == typeof(Color))
                    return textBox.BackColor;
                return textBox.Text;
            }
            else
            {
                try
                {
                    switch (target)
                    {
                        case ComboBox cb: return cb.SelectedItem;
                        case TrackBar tb: return tb.Value / fs.TrackBarMultiplier;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Strange error");
                    Console.WriteLine(ex);
                }
            }
            return null;
        }
       
        
        public void SetFieldValue(string key, object value)
        {
            var target = FieldLocator.LocateName(key);
            if (target == null)
                return;
            if (value.GetType() == typeof(Int32) || value.GetType() == typeof(double))
            {
                target.Field.Text = $"{value}";
                return;
            }
            if (value.GetType() == typeof(Color))
            {
                target.Field.BackColor = (Color)value;
                target.Field.ForeColor = (Color)value;
                return;
            }
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to set field value");
                Console.WriteLine(ex);
            }
        }

        #endregion



        #region Page

        public void RenamePage(int pageNumber, string newName)
        {
            var target = pageNumber < 0 ? Owner : PageLocator.LocateIndex(pageNumber);
            target.Name = newName;

        }


        public void AddPage()
        {

            if (Owner.NextPage != null)
            {
                Console.WriteLine("Failed to create page, this is not the last page");
                return;
            }
            Button btn;
            // NextPage from Owner setup
            btn = EditMiscControl(MiscControl.NextPage) as Button;
            btn.Click += new EventHandler(Owner.OnPageUp);
            Owner.NextPage = InteractiveEditor.GeneratePage(Owner);           

            // PrevPage from NextPage setup            
            btn = Owner.NextPage.Modify.EditMiscControl(MiscControl.PrevPage) as Button;
            btn.Click += new EventHandler(Owner.NextPage.OnPageDown);
            Owner.NextPage.PrevPage = Owner;
        }


        public void PrependPage()
        {
            PageLocator.LocateLast().Modify.RemovePage();
        }


        public void RemovePage()
        {
            if (Owner.PrevPage == null || Owner.NextPage != null)
            {
                return;
            }
            Owner.Dispose();
            var btn = EditMiscControl(MiscControl.NextPage) as Button;
            btn.Click -= new EventHandler(Owner.OnPageUp);
            Owner.PrevPage.Visible = true;
            Owner.PrevPage.NextPage = null;
            Owner.Fields.Clear();

        }

        #endregion



        #region Other

        public Control EditMiscControl(MiscControl target)
        {
            return Owner.MiscControls[target];
        }

        #endregion








    }
}
