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
    public interface ILocatorService : ILocatorService<InteractiveEditor> { }
    public class PageLocatorService : ILocatorService
    {



        #region Properties
        
        public IOBServiceProvider Provider { get; set ; }

        #endregion



        #region Methods
        
        public int Count
        {
            get
            {
                var c = 0;
                foreach (InteractiveEditor iob in Provider.Request<PageLocatorService>())
                    c++;
                return c;                                
            }
        }
             
        public InteractiveEditor[] ToArray()
        {
            InteractiveEditor[] newArr = new InteractiveEditor[Provider.Request<PageLocatorService>().Count];
            var index = -1;
            foreach (InteractiveEditor iob in Provider.Request<PageLocatorService>())            
                newArr[++index] = iob;            
            return newArr;
        }

        public List<InteractiveEditor> ToList()
        {
            List<InteractiveEditor> newList = new List<InteractiveEditor>();
            foreach (InteractiveEditor iob in Provider.Request<PageLocatorService>())            
                newList.Add(iob);
            return newList;
        }

        public InteractiveEditor LocateIndex(int targetIndex)
        {
            if (targetIndex < 0)
                throw new ArgumentException("pageIndex must be a positive integer.", "pageIndex");
            var actualIndex = -1;
            foreach (InteractiveEditor iob in Provider.Request<PageLocatorService>())            
                if (++actualIndex == targetIndex)
                    return iob;                                        
            return null;
        }

        #endregion 



        #region High data exposition methods      

        public InteractiveEditor LocateName(string targetName)
        {
            foreach (InteractiveEditor iob in Provider.Request<PageLocatorService>())
                if (targetName.Equals(iob.Name))
                    return iob;
            return null;
        }

        public InteractiveEditor LocateFirst()
        {
            var actualPage = Provider.Owner;
            while (actualPage.PrevPage != null)            
                actualPage = actualPage.PrevPage;            
            return actualPage;
        }

        public InteractiveEditor LocateLast()
        {
            var actualPage = Provider.Owner;
            while (actualPage.NextPage != null)            
                actualPage = actualPage.NextPage;            
            return actualPage;
        }

        public IEnumerator GetEnumerator()
        {            
            var actualPage = LocateFirst();
            while (actualPage != null)
            {
                yield return actualPage;
                actualPage = actualPage.NextPage;
            }
        }
        
        #endregion 



        #region obsolete or unavailable content

        [Obsolete("This property is obsolete for PageLocatorService. Use \"Count\" Property instead. (Default return: -1)", true)]
        public int CountAll => -1;
        [Obsolete("IEnumerable GetEnumeratorEx() have the same effect of IEnumerable GetEnumerator() for PageLocatorService. Please use IEnumerable GetEnumerator() instead.", true)]
        public IEnumerable GetEnumeratorEx()
        {
            var actualPage = LocateFirst();
            while (actualPage != null)
            {
                yield return actualPage;
                actualPage = actualPage.NextPage;
            }
        }
        [Obsolete("This method is not available for the PageLocatorService", true)]
        public InteractiveEditor LocatePredicate<argT>(argT targetValue = null, Func<argT, InteractiveEditor, bool> predict = null) where argT : class
        {
            return null;
        }
        #endregion



    }

}