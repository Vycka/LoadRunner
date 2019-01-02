using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    public class ValueMetric<T> : ValuesMetric<T>
    {
        public ValueMetric(string name, Func<T, object> selector)
            : base(data => new[] { new Val(name, selector(data)) })
        {
        }
    }

    public class ValuesMetric<T> : IMetric<T>
    {
        private readonly Func<T, IEnumerable<Val>> _selector;
        private bool _done;

        public ValuesMetric(Func<T, IEnumerable<Val>> selector)
        {
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));

            ColumnNames = new string[0];
            Values = new object[0];
            _done = false;
        }

        public IMetric<T> CreateNew()
        {
            return new ValuesMetric<T>(_selector);
        }

        public void Add(T data)
        {
            if (!_done)
            {
                Val[] columns = _selector(data).ToArray();

                ColumnNames = columns.Select(kv => kv.Key).ToArray();
                Values = columns.Select(kv => kv.Value).ToArray();

                _done = true;
            }
        }

        public string[] ColumnNames { get; private set; }
        public object[] Values { get; private set; }
    }

    public class Val
    {
        public Val(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key;
        public object Value;
    }
}