using System.Collections.Generic;

namespace Viki.LoadRunner.Engine.Aggregators.Utils
{
    /// <summary>
    /// Tool for learning sort order of columns
    /// </summary>
    public class OrderLearner
    {
        private List<string> _nameOrder;
        public IReadOnlyList<string> LearnedOrder => _nameOrder; 

        public OrderLearner()
        {
            Reset();
        }

        public void Reset()
        {

            _nameOrder = new List<string>();
        }

        public void Learn(string[] names)
        {

            string previousName = "";
            foreach (string name in names)
            {
                if (!_nameOrder.Contains(name))
                { 
                    if (_nameOrder.Count == 0)
                    {
                        _nameOrder.Add(name);
                    }
                    else
                    {
                        int insertPosition = _nameOrder.FindIndex(s => s == previousName) + 1;

                        if (insertPosition == 0 || insertPosition == _nameOrder.Count)
                            _nameOrder.Add(name);
                        else
                            _nameOrder.Insert(insertPosition, name);
                    }
                }

                previousName = name;
            }
            
        }
    }
}