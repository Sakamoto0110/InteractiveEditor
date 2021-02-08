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
    public class FieldLocatorService : ILocatorService<Fieldset>
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

        public int CountAll
        {
            get
            {
                var c = 0;
                foreach (InteractiveEditor iob in Provider.Request<PageLocatorService>())                
                    c += iob.Fields.Count;
                return c;
            }
        }

        public Fieldset[] ToArray()
        {
            Fieldset[] newArr = new Fieldset[(Provider.Bytes[0] ^ 0x01) == 0 ?
                                             Provider.Request<FieldLocatorService>().CountAll :
                                             Provider.Request<FieldLocatorService>().Count];
            var index = -1;
            if ((Provider.Bytes[0] ^ 0x01) == 0)
                foreach (InteractiveEditor iob in Provider.Request<PageLocatorService>())
                    foreach (Fieldset fs in iob.ServiceProvider.Request<FieldLocatorService>())                    
                        newArr[--index] = fs;                    
            else
                foreach (Fieldset fs in Provider.Request<FieldLocatorService>())                
                    newArr[--index] = fs;                
            return newArr;
        }

        public List<Fieldset> ToList()
        {
            List<Fieldset> newList = new List<Fieldset>();
            if ((Provider.Bytes[0] ^ 0x01) == 0)            
                foreach (InteractiveEditor iob in Provider.Request<PageLocatorService>())
                    foreach (Fieldset fs in Provider.Request<FieldLocatorService>())
                        newList.Add(fs);            
            else            
                foreach (Fieldset fs in Provider.Request<FieldLocatorService>())
                    newList.Add(fs);                                            
            return newList;
        }

        public Fieldset LocateName(string targetName)
        {
            if ((Provider.Bytes[0] ^ 0x01) == 0)
            {
                foreach (Fieldset field in Provider.Request<FieldLocatorService>().GetEnumeratorEx())
                    if (targetName.Equals(field.Name))
                        return field;
            }
            else
                foreach (Fieldset vf in Provider.Request<FieldLocatorService>())
                    if (vf.Name.Equals(targetName))
                        return vf;
            return null;
        }

        public Fieldset LocateIndex(int targetIndex)
        {
            int actualFieldIndex = -1;
            if ((Provider.Bytes[0] ^ 0x01) == 0)
            {
                foreach (Fieldset field in Provider.Request<FieldLocatorService>().GetEnumeratorEx())
                    if (++actualFieldIndex == targetIndex)
                        return field;
            }
            else                          
                foreach (Fieldset field in Provider.Request<FieldLocatorService>())                
                    if (++actualFieldIndex == targetIndex)                    
                        return field;
            return null;
        }

        #endregion



        #region High data exposition methods      

        public Fieldset LocatePredicate<argT>(argT targetValue = null, Func<argT, Fieldset, bool> predict = null) where argT : class
        {
            if ((Provider.Bytes[0] ^ 0x01) == 0)
            {
                foreach (Fieldset field in Provider.Request<FieldLocatorService>().GetEnumeratorEx())
                    if (predict(targetValue, field))
                        return field;
            }
            else
                foreach (Fieldset field in Provider.Request<FieldLocatorService>())
                    if (predict(targetValue, field))
                        return field;
            return null;
        }


        public Fieldset LocateFirst()
        {
            return (Provider.Bytes[0] ^ 0x01) == 0 ? Provider.Request<PageLocatorService>().LocateFirst().Fields[0] : Provider.Owner.Fields[0];
        }


        public Fieldset LocateLast()
        {
            return (Provider.Bytes[0] ^ 0x01) == 0 ? Provider.Request<PageLocatorService>().LocateLast().Fields[Provider.Request<PageLocatorService>().LocateLast().Fields.Count - 1] : Provider.Owner.Fields[Provider.Owner.Fields.Count - 1];
        }


        public IEnumerable GetEnumeratorEx()
        {
            foreach (InteractiveEditor iob in Provider.Request<PageLocatorService>())            
                foreach (Fieldset vf in iob.Fields)                
                    yield return vf;                            
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
