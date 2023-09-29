using System.Collections;
using System.Collections.Generic;

namespace Parametrics;

public class ParametricCollection : IEnumerable<ParametricFunc>
{
    List<ParametricFunc> _funcs = new();

    public void Add(ParametricFunc func)
    {
        _funcs.Add(func);
    }

    public IEnumerator<ParametricFunc> GetEnumerator()
    {
        return _funcs.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable) _funcs).GetEnumerator();
    }
}