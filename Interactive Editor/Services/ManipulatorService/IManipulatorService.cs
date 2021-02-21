using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Editor.Modifiers;

namespace Editor.Services
{
    public interface IManipulatorService
    {

        #region Fields
        
        void AddField(Type U, string fieldName, FieldFlags flags = FieldFlags.None);

        void AddField<U>(string fieldName, FieldFlags flags = FieldFlags.None);

        /// <summary>
        /// Used for get specific fields
        /// </summary>
        /// <param name="key">
        /// Is the <b>name</b> of the <b>Fieldset</b>
        /// </param>        
        /// <returns>The Control that is storing the value </returns>
        /// <remarks>If the key is null or empty, will return the last created field.</remarks>
        Control EditField(string key = null);


        Label EditFieldLabel(string key = null);


        void ToggleFieldVisible(bool newVal, string key = null);
        

        void SetDeltaMultiplier(double value,string key = null);
        

        void SetHSliderMultiplier(double value, string key = null);


        void SetFieldValue(string key, object value);
        
        
        void EditFieldValueByVariableName(string key, object value);
        
        
        object GetFieldValue<O>(string key);
                
        #endregion        

        #region Pages
        
        void RenamePage(int pageNumber, string newName);


        void AddPage();


        void PrependPage();
        
        
        void RemovePage();
                       
        #endregion

        #region Other

        Control EditMiscControl(MiscControl target);

        #endregion

    }
}
