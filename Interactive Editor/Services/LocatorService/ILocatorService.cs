using Editor.Fields;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Editor.Services

{
    public interface IFieldLocatorService : _IService, IEnumerable
    {

        int Count { get; }
        List<Fieldset> ToList();
        Fieldset[] ToArray();
        Fieldset LocateIndex(int targetIndex = -1);
        Fieldset LocateName(string targetName);
        Fieldset LocatePredicate<TValue>(TValue targetValue = null, Func<TValue, Fieldset, bool> predicate = null) where TValue : class;
        Fieldset LocateFirst();
        Fieldset LocateLast();

    }
}
