using Editor.Fields;
using Editor.Services;
using Editor.Services.BinderService.Mapping;
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
        public Inspector Owner => Provider.Owner;


        private FieldLocatorService FieldLocator => Provider.Request<FieldLocatorService>();

        #region Field


        public void AddField(Type U, string fieldName, FieldFlags flags = FieldFlags.None)
        {
            if (FieldLocator.LocateName(fieldName) is null)
            {
                
            }
            Owner.Fields.Add(new Fieldset(
                               Owner,
                               Owner.Fields.Count + 1,
                               fieldName,
                               U,
                               0,
                                (Owner.Horizontal_Spacing * Owner.Fields.Count) + (Owner.Fields.Count * Owner.FieldHeight),
                               flags));

        }


        public void AddField<U>(string fieldName, FieldFlags flags = FieldFlags.None)
        {
            if(FieldLocator.LocateName(fieldName) is null)
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

        
        public Control EditField(string key = null)
        {
            var target = key is null ? FieldLocator.LocateLast() : FieldLocator.LocateName(key);
            return target?.Field;
        }


        public Label EditFieldLabel(string key = null)
        {            
            return (key is null ? FieldLocator.LocateLast() : FieldLocator.LocateName(key))?.Label;
        }


        public void SetDeltaMultiplier(double value, string key = null)
        {
            var target = key is null ? FieldLocator.LocateLast() : FieldLocator.LocateName(key);
            if (target != null)
                target.Delta = value;
        }


        public void SetHSliderMultiplier(double value, string key = null)
        {
            var target = key is null ? FieldLocator.LocateLast() : FieldLocator.LocateName(key);
            if (target != null)
                target.HSliderMultiplier = value;
        }


        public void ToggleFieldVisible(bool newVal, string key = null)
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







        public void BuildFieldsForTypeByMapping(MapObject map)
        {
            int c = 0;
            int totalValues = map.Values.Count;
            BindingArgs[] argsArr = map.Values.ToArray();

            for(int i = 0; i < totalValues; i++)
            {
                BindingArgs args = argsArr[i];
                
                
                AddField(args.FieldSet_FieldType, args.FieldSet_Name);
                var f = FieldLocator.LocateLast();
                f.Label.Text = args.FieldSet_Text ?? args.FieldSet_Name;

            }

            

          

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
