using System;
using System.Collections.Generic;

namespace NBasis.Types
{
    public interface ITypeFinder
    {
        /// <summary>
        /// Retrieves every loaded type known by the finder
        /// </summary>
        IEnumerable<Type> AllLoadedTypes { get; }

        /// <summary>Finds types assignable from of a certain type in the finder</summary>
        /// <param name="requestedType">The type to find.</typeparam>
        IEnumerable<Type> GetDerivedTypes<TBase>() where TBase : class;

        /// <summary>
        /// Get all of the implementations of a given interface
        /// </summary>        
        IEnumerable<Type> GetInterfaceImplementations(Type interfaceType);

        /// <summary>
        /// Get all of the implementations of a given interface
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>        
        IEnumerable<Type> GetInterfaceImplementations<TInterface>() where TInterface : class;
    }
}
