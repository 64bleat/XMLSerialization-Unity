using System;

namespace Serialization
{
    /// <summary> Flags a class as an xml surrogate for the type <c>surrogateOf</c> </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class XMLSurrogateAttribute : Attribute
    {
        public readonly Type componentType;

        public XMLSurrogateAttribute(Type surrogateOf)
        {
            componentType = surrogateOf;
        }
    } 
}