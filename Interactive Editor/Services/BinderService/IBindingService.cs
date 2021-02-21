using Editor.Events;
using Editor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Editor.InteractiveEditor;
using static Editor.Modifiers;

namespace Editor.Services
{
    public interface IBindingService : _IService 
    {

        event EventHandler<BinderEventArgs> BindStarted;
        event EventHandler<BinderEventArgs> BindFinished;


        void BindToObject(object Instance, bool multiBind = false, Func<object, object, object, object> bruteForce = null);
        void UnbindObject();
        bool CheckIfFieldExists(string varName);
        void BindToVariable(string key, string varName, CapFunction f = null, object capParms = null);







        /// <summary>
        ///
        /// <param name="targetFilter">
        /// ( Optional ) Invoke a function to filter the available variables, can be blacklisted or whitelisted        
        /// </param>
        ///
        /// <param name="filterOperation">        
        /// ( Optional ) Sets the operation of the filter ( 1 = Whitelist , -1 = Blacklist )
        /// <para> Default: Blacklist </para>
        /// </param>
        /// 
        /// </summary>
        void BindToTypo(string[] targetFilter = null, FilterMode filterOperation = FilterMode.Blacklist);




        /// <summary>
        /// Binds the entire Editor to a specific Type, using the available variables list to search fields
        ///
        /// <param name="FTypeToControlMapping">
        /// Uses a custom function to map the Type of the variable to be binded and a Control to display the value
        /// <para> Type F(FieldInfo arg0) </para>
        /// <para> arg0: The metadata of the target variable </para>
        /// <para> return: The TYPE of the Control ( exemple: return typeof(TextBox); ) </para>
        /// <para>* If the return type is equals to null ( return null; ), the default value will be applied *</para>     
        /// </param>
        /// 
        /// <param name="targetFilter">
        /// ( Optional ) MaskArray, can be blacklisted or whitelisted
        /// </param>
        /// 
        /// <param name="filterOperation">
        /// ( Optional ) Sets the operation of the filter ( 1 = Whitelist , -1 = Blacklist )
        /// <para> Default: Blacklist </para>
        /// </param>
        /// 
        /// </summary>
        void BindToTypo(BindingService.BindingConfigurator FTypeToControlMapping, string[] targetFilter = null, FilterMode filterOperation = FilterMode.Blacklist);

     


    }
}
