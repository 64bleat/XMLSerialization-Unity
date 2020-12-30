namespace Serialization
{
    /// <summary> Interface for a serialization surrogate </summary>
    /// <remarks> The ISurrogate interface does not need to be implemented in the same chass
    /// that is being serialized. This would be impossible for GameObjects and Components. </remarks>
    public interface ISurrogate
    {
        /// <summary> Creates a serialization surrogate for the specified class. </summary>
        /// <remarks> o must be typecast to the specified class within the method.</remarks>
        /// <param name="o"> the object that will be converted to a serialization surrogate </param>
        /// <returns> a generated serialization surrogate for o</returns>
        public abstract ISurrogate Serialize(dynamic o);

        /// <summary> Applies info in a serialization surrogate onto an instance of the specified class </summary>
        /// <remarks> The instance must already exist. <c>Deserialize</c> usually doesn't create the instance. </remarks>
        /// <param name="o"> the object that will receive this serialization surrogate's data </param>
        /// <returns> (This should be changed to just return void) </returns>
        public abstract ISurrogate Deserialize(dynamic o);
    }
}
