﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serialization
{
    /// <summary> XML Surrogates represent non-serializeable classes in a serializeable form.
    /// They also contain methods for describing how to serialize and deserialize
    /// these classes.</summary>
    /// <remarks> Some classes, like GameObject, require <c>Deserialize</c> to instantiate
    /// the serialized components before deserializing them. </remarks>
    [Serializable]
    public abstract class XMLSurrogate
    {
        private static bool initialized = false;
        private static readonly Dictionary<Type, Type> surrogateMap = new Dictionary<Type, Type>();

        /// <summary> Creates a serialization surrogate for the specified class. </summary>
        /// <remarks> o must be typecast to the specified class within the method.</remarks>
        /// <param name="o"> the object that will be converted to a serialization surrogate </param>
        /// <returns> a generated serialization surrogate for o</returns>
        public abstract XMLSurrogate Serialize(dynamic o);

        /// <summary> Applies info in a serialization surrogate onto an instance of the specified class </summary>
        /// <remarks> The instance must already exist. <c>Deserialize</c> usually doesn't create the instance. </remarks>
        /// <param name="o"> the object that will receive this serialization surrogate's data </param>
        /// <returns> (This should be changed to just return void) </returns>
        public abstract XMLSurrogate Deserialize(dynamic o);

        /// <summary>  Loads all surrogates and their associations into memory </summary>
        private static void Initialize()
        {
            Type xmlSurrogateAttributeType = typeof(XMLSurrogateAttribute);

            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type surrogateType in assembly.GetTypes())
                    if (surrogateType.GetCustomAttribute(xmlSurrogateAttributeType, false)
                        is XMLSurrogateAttribute surrogateAttribute
                        && surrogateAttribute.componentType != null
                        && !surrogateMap.ContainsKey(surrogateAttribute.componentType))
                        surrogateMap.Add(surrogateAttribute.componentType, surrogateType);

            initialized = true;
        }

        /// <summary> Returns an array listing all available surrogate types. </summary>
        public static Type[] GetLoadedSurrogates()
        {
            if (!initialized)
                Initialize();

            return surrogateMap.Values.ToArray();
        }

        /// <summary> Returns an instance of the surrogate associated with the provided type </summary>
        public static XMLSurrogate InstantiateSurrogateFor(Type type)
        {
            if (!initialized)
                Initialize();

            if (GetSurrogateTypeFor(type) is Type t && t != null)
                return Activator.CreateInstance(t) as XMLSurrogate;
            else
                return null;
        }

        /// <summary> Returns the surrogate type associated with the provided type </summary>
        public static Type GetSurrogateTypeFor(Type componentType)
        {
            if (!initialized)
                Initialize();

            if (surrogateMap.TryGetValue(componentType, out Type surrogateType))
                return surrogateType;
            else
                return null;
        }

        public static bool TryGetComponentType(Type surrogateType, out Type componentType)
        {
            Type attributeType = typeof(XMLSurrogateAttribute);
            XMLSurrogateAttribute surrogate = Attribute.GetCustomAttribute(surrogateType, attributeType) as XMLSurrogateAttribute;

            componentType = surrogate?.componentType ?? default;

            return componentType?.Equals(default) ?? false;
        }
    }
}
