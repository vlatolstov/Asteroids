using System;
using System.Collections.Generic;
using System.Linq;

namespace _Project.Runtime.Utils
{
    public static class TypeHelpers
    {
        public static IEnumerable<Type> GetTypesImplementingInterface<TInterface>()
        {
            var interfaceType = typeof(TInterface);
            
            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException("TInterface must be an interface type.");
            }
            
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly =>
                    assembly.GetTypes()
                        .Where(type =>
                            !type.IsAbstract && !type.IsInterface && interfaceType.IsAssignableFrom(type)));
        }
    }
}