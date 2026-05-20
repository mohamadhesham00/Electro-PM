using System.Reflection;

namespace Application.Extensions
{
    public static class MappingExtensions
    {
        public static void PatchInto<TSource, TDestination>(this TSource source, TDestination destination)
        {
            var sourceProperties = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var destType = typeof(TDestination);

            foreach (var sourceProp in sourceProperties)
            {
                var sourceValue = sourceProp.GetValue(source);

                // Skip if the property wasn't provided (null or empty string)
                if (sourceValue == null || (sourceValue is string s && string.IsNullOrEmpty(s)))
                    continue;

                // Find the matching property on the destination entity
                var destProp = destType.GetProperty(sourceProp.Name, BindingFlags.Public | BindingFlags.Instance);

                if (destProp != null && destProp.CanWrite)
                {
                    destProp.SetValue(destination, sourceValue);
                }
            }
        }
    }
}
