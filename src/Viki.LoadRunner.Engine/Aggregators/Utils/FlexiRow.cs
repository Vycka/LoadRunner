using System;
using System.Collections;
using System.Collections.Generic;

namespace Viki.LoadRunner.Engine.Aggregators.Utils
{
    public class FlexiRow<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        #region Fields

        private readonly Func<TValue> _valueBuilderFunc;
        private readonly Dictionary<TKey, TValue> _row = new Dictionary<TKey, TValue>();

        #endregion

        #region Constructor

        public FlexiRow(Func<TValue> valueBuilderFunc)
        {
            if (valueBuilderFunc == null)
                throw new ArgumentNullException(nameof(valueBuilderFunc));

            _valueBuilderFunc = valueBuilderFunc;
        }

        #endregion

        #region public functionality 

        public void Touch(TKey key)
        {
            if (!_row.ContainsKey(key))
                _row.Add(key, _valueBuilderFunc());
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (!_row.TryGetValue(key, out value))
                {
                    value = _valueBuilderFunc();
                    _row.Add(key, value);
                }

                return value;
            }
            set { _row[key] = value; }
        }

        public int Count => _row.Count;

        public void Clear()
        {
            _row.Clear();
        }

        public IEnumerable<TKey> Keys => _row.Keys;
        public IEnumerable<TValue> Values => _row.Values;

        #endregion
        
        #region IEnumerable

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _row.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();
        }

        #endregion
    }
}