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

        void AddField<U>(string fieldName, FieldFlags flags = FieldFlags.None);


        Control EditField(string key = null, bool GetPanel = false);


        Label EditFieldLabel(string key = null);


        void ToggleFieldVisible(string key, bool newVal);
        

        void SetDeltaMultiplier(string key = null, double value = 1);
        

        void SetHSliderMultiplier(string key = null, double value = 1);


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
