using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace exd.World.ResourceCostHelper
{
    public static class ResourceProperties
    {
        private struct PropertiesType
        {
            internal double Weight;
        }

        private static Dictionary<ResourceType, PropertiesType> Properties = new Dictionary<ResourceType, PropertiesType>();

        public static void Initialize()
        {
            // already initialized?
            if (Properties.Any())
                return;

            // parse the resources enum for properties from attributes 
            // this is to avoid potentially costly runtime reflection calls
            foreach (ResourceType resourcevalue in Enum.GetValues(typeof(ResourceType)))
            {
                var resourcefield = typeof(ResourceType).GetMember(resourcevalue.ToString())[0];

                double weight = 0;
                var weightattribute = resourcefield.GetCustomAttributes(typeof(ResourceWeightAttribute), true);
                if (weightattribute != null && weightattribute.Any())
                    weight = ((ResourceWeightAttribute)weightattribute[0]).Weight;

                Properties.Add(resourcevalue, new PropertiesType
                {
                    Weight = weight
                });
            }
        }

        public static double GetResourceWeight(ResourceType resource) { return Properties[resource].Weight; }
    }
}
