using Editor.Fields;
using Editor.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Services
{
    public class FieldLocatorService : IFieldLocatorService
    {



        #region Properties
        
        public IOBServiceProvider Provider { get; set; }
 

        #endregion



        #region Methods
        
        public int Count
        {
            get
            {
                return Provider.Owner.Fields.Count;
            }
        }

       

        public Fieldset[] ToArray()
        {
            Fieldset[] newArr = new Fieldset[Provider.Request<FieldLocatorService>().Count];
            var index = -1;

            foreach (Fieldset fs in Provider.Request<FieldLocatorService>())
                newArr[--index] = fs;
            return newArr;
        }

        public List<Fieldset> ToList()
        {
            List<Fieldset> newList = new List<Fieldset>();

            foreach (Fieldset fs in Provider.Request<FieldLocatorService>())
                newList.Add(fs);
            return newList;
        }

        public Fieldset LocateName(string targetName)
        {

            foreach (Fieldset vf in Provider.Request<FieldLocatorService>())
                if (vf.Name.Equals(targetName))
                    return vf;
            return null;
        }

        public Fieldset LocateIndex(int targetIndex)
        {
            int actualFieldIndex = -1;

            foreach (Fieldset field in Provider.Request<FieldLocatorService>())
                if (++actualFieldIndex == targetIndex)
                    return field;
            return null;
        }

        #endregion



        #region High data exposition methods      

        public Fieldset LocatePredicate<argT>(argT targetValue = null, Func<argT, Fieldset, bool> predict = null) where argT : class
        {

            foreach (Fieldset field in Provider.Request<FieldLocatorService>())
                if (predict(targetValue, field))
                    return field;
            return null;
        }


        public Fieldset LocateFirst()
        {
            return Provider.Owner.Fields[0];
        }


        public Fieldset LocateLast()
        {
            return Provider.Owner.Fields[Provider.Owner.Fields.Count - 1];
        }


      


        public IEnumerator GetEnumerator()
        {
            foreach (Fieldset vf in Provider.Owner.Fields)            
                yield return vf;            
        }


        #endregion



        #region obsolete or unavailable content

        
        #endregion



    }
}
