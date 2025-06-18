using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.Core.Models.Wizard
{
    public class TeachingContext
    {
        public Dictionary<string, object> Data { get; } = new();
        public int CurrentStepIndex { get; set; }

        private readonly Dictionary<string, object> _data = new();

        public void Set<T>(string key, T value)
        {
            _data[key] = value!;
        }

        public T Get<T>(string key)
        {
            return _data.TryGetValue(key, out var value) ? (T)value : throw new KeyNotFoundException(key);
        }

        public bool Has(string key)
        {
            return _data.ContainsKey(key);
        }
    }
}
