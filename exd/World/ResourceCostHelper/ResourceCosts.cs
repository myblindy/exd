using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exd.World.ResourceCostHelper
{
    public class ResourceCosts : IEnumerable<KeyValuePair<ResourceType, double>>
    {
        private Dictionary<ResourceType, double> Resources = new Dictionary<ResourceType, double>();

        public void Add(ResourceType type, double qty)
        {
            if (Resources.ContainsKey(type))
                Resources[type] += qty;
            else
                Resources.Add(type, qty);
        }

        public void Add(ResourceCosts costs)
        {
            foreach (var kvp in costs)
                Add(kvp.Key, kvp.Value);
        }

        public double this[ResourceType type]
        {
            get
            {
                return Resources.ContainsKey(type) ? Resources[type] : 0;
            }
        }

        public IEnumerator<KeyValuePair<ResourceType, double>> GetEnumerator()
        {
            return Resources.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Resources.GetEnumerator();
        }
    }
}
