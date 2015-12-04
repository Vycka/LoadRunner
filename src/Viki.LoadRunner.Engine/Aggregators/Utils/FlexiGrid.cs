using System;
using System.Collections;
using System.Collections.Generic;
#pragma warning disable 1591

namespace Viki.LoadRunner.Engine.Aggregators.Utils
{
    public class FlexiGrid<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        #region Fields

        private readonly Func<TValue> _valueBuilderFunc;
        private readonly Dictionary<TKey, TValue> _grid = new Dictionary<TKey, TValue>();

        #endregion

        #region Constructor

        public FlexiGrid(Func<TValue> valueBuilderFunc)
        {
            if (valueBuilderFunc == null)
                throw new ArgumentNullException(nameof(valueBuilderFunc));

            _valueBuilderFunc = valueBuilderFunc;
        }

        #endregion

        #region public functionality 

        public void Touch(TKey key)
        {
            if (!_grid.ContainsKey(key))
                _grid.Add(key, _valueBuilderFunc());
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (!_grid.TryGetValue(key, out value))
                {
                    value = _valueBuilderFunc();
                    _grid.Add(key, value);
                }

                return value;
            }
            set { _grid[key] = value; }
        }

        public int Count => _grid.Count;

        public void Clear()
        {
            _grid.Clear();
        }

        public IEnumerable<TKey> Keys => _grid.Keys;
        public IEnumerable<TValue> Values => _grid.Values;

        #endregion
        
        #region IEnumerable

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _grid.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();
        }

        #endregion
    }
}