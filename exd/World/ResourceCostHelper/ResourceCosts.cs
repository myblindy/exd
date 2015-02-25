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

        public double TotalWeight
        {
            get
            {
                return -Resources.Sum(kvp => ResourceProperties.GetResourceWeight(kvp.Key) * kvp.Value);
            }
        }

        public ResourceCosts SplitToWeight(double maxweight)
        {
            var newstack = new ResourceCosts();
            var cummweight = 0.0;

        redo:
            foreach (var kvp in Resources)
            {
                var unitweight = ResourceProperties.GetResourceWeight(kvp.Key);
                var qtytransfer = 0.0;

                if (unitweight * -kvp.Value + cummweight > maxweight)
                    qtytransfer = (maxweight - cummweight) / unitweight;

                if (qtytransfer > 0)
                {
                    Add(kvp.Key, qtytransfer);
                    newstack.Add(kvp.Key, -qtytransfer);
                    goto redo;
                }
            }

            return newstack;
        }

        public bool SufficientlyCovers(ResourceCosts costs)
        {
            foreach (var kvp in costs)
                if (Math.Abs(this[kvp.Key]) < Math.Abs(kvp.Value))
                    return false;

            return true;
        }

        public static ResourceCosts operator +(ResourceCosts a, ResourceCosts b)
        {
            var clone = new ResourceCosts();
            clone.Add(a);
            clone.Add(b);

            return clone;
        }

        public bool StillRequired
        {
            get
            {
                foreach (var kvp in Resources)
                    if (kvp.Value > 0)
                        return true;

                return false;
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
