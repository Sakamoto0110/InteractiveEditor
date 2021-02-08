using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Services

{
    public interface ILocatorService<TInstance>: _IService, IEnumerable
    {

        int Count { get; }
        int CountAll { get; }
        List<TInstance> ToList();
        TInstance[] ToArray();
        TInstance LocateIndex(int targetIndex = -1);
        TInstance LocateName(string targetName);
        TInstance LocatePredicate<TValue>(TValue targetValue = null, Func<TValue, TInstance, bool> predicate = null) where TValue : class;
        TInstance LocateFirst();
        TInstance LocateLast();
        IEnumerable GetEnumeratorEx();

    }
}
