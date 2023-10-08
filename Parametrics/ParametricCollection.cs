using System.Collections;
using System.Collections.Generic;

namespace Parametrics;

public class ParametricCollection : IEnumerable<ParametricSegment>
{
    List<ParametricSegment> _funcs = new();

    public void Add(ParametricSegment func)
    {
        _funcs.Add(func);
    }

    public IEnumerator<ParametricSegment> GetEnumerator()
    {
        return _funcs.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable) _funcs).GetEnumerator();
    }
}