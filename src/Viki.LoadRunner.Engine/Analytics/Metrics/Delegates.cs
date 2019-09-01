using System.Collections.Generic;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    public delegate Val ValSelectorDelegate<in TData>(TData data);
    public delegate IEnumerable<Val> ValuesSelectorDelegate<in TData>(TData data);
    public delegate object ObjectSelectorDelegate<in TData>(TData data);
    public delegate long LongSelectorDelegate<in TData>(TData data);
    public delegate double DoubleSelectorDelegate<in TData>(TData data);
    public delegate bool BoolSelectorDelegate<in TData>(TData data);
}