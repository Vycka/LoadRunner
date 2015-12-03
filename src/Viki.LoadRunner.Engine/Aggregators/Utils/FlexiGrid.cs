using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Viki.LoadRunner.Engine.Aggregators.Utils
{
    public class FlexiGrid<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        #region Fields

        private readonly Func<TValue> _builderFunc;
        private readonly Dictionary<TKey, TValue> _grid = new Dictionary<TKey, TValue>();

        #endregion

        #region Constructor

        public FlexiGrid(Func<TValue> builderFunc)
        {
            if (builderFunc == null)
                throw new ArgumentNullException(nameof(builderFunc));

            _builderFunc = builderFunc;
        }

        #endregion

        #region public functionality 

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (!_grid.TryGetValue(key, out value))
                {
                    value = _builderFunc();
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