using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace jhaCC.Models
{
    public class KeyAccumulator
    {
        // simple class to allow us to accumulate occurances of particular key values
        // we use this to accumulate emojis/hashtags/domains

        // string/int => key (i.e. hashtag), count
        ConcurrentDictionary<string, int> _KeyAccumulator = new ConcurrentDictionary<string, int> { };

        private readonly IConfiguration config;

        public KeyAccumulator(IConfiguration config)
        {
            this.config = config;
        }

        public void Clear()
        {
            _KeyAccumulator = new ConcurrentDictionary<string, int> { };
        }
        public int Increment(String value)
        {
            value = value.ToUpper();

            if (_KeyAccumulator.ContainsKey(value))
            {
                _KeyAccumulator[value] = _KeyAccumulator[value] + 1;
            }
            else
            {
                if (!_KeyAccumulator.TryAdd(value, 1))
                {
                    // key already existed, so, instead of priming with a value of 1,
                    // we will accumulate - I think this is prone to bugs though
                    _KeyAccumulator.TryUpdate(value, _KeyAccumulator[value] + 1, _KeyAccumulator[value]);
                };
            }
            return _KeyAccumulator[value];
        }

        public int Increment(List<string> values)
        {
            int itemsAdded = 0;

            foreach (string value in values)
            {
                itemsAdded += Increment(value);
            }

            return itemsAdded;
        }

        public IEnumerable<KeyValuePair<string, int>> GetTopValue()
        {
            List<KeyValuePair<string, int>> myList = _KeyAccumulator.ToList();

            if (_KeyAccumulator.Count > 0)
            {
                myList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                return myList.Take(config.GetValue<int>("Services:Reporting:AutoReportTopCount",5));
            }
            else
            {
                return (IEnumerable<KeyValuePair<string, int>>)myList;
            }
        }

        public int Count()
        {
            return _KeyAccumulator.Count;
        }
    }
}
