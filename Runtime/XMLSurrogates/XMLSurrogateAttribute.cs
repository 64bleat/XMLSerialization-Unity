using System;

namespace Serialization
{
    /// <summary> Assigns the class as a serialization surrogate of the provided class </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class XMLSurrogateAttribute : Attribute
    {
        public readonly Type componentType;

        public XMLSurrogateAttribute(Type surrogateOf)
        {
            componentType = surrogateOf;
        }
    } 
}