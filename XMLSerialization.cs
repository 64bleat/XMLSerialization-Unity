using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace Serialization
{
    /// <summary> Methods for serializing and deserializing any serializeable type. </summary>
    public static class XMLSerialization
    {
        private const string filetype = ".txt";
        private const string saveFolder = "/saves/";

        /// <summary> Saves an instance of a type and its heirarchy into an xml file </summary>
        /// <typeparam name="T"> T must be XML-serializeable </typeparam>
        /// <param name="saveData"> the object that will be serialized</param>
        /// <param name="filePath"> file path relative to <c>Application.persistentDataPath</c></param>
        public static void Save<T>(T saveData, string filePath)
        {
            string directory = Application.persistentDataPath + saveFolder;
            string path = directory + filePath + filetype;
            Directory.CreateDirectory(directory);
            FileStream stream = File.Create(path);
            XmlSerializer serializer = new XmlSerializer(typeof(T), XMLSurrogate.GetLoadedSurrogates());
            serializer.Serialize(stream, saveData);
            stream.Close();
        }

        /// <summary> Saves an instance of a type and its heirarchy into an xml file </summary>
        /// <typeparam name="T"> specifies the type being de-serialized </typeparam>
        /// <param name="filePath"> file path relative to <c>Application.persistentDataPath</c></param>
        public static T Load<T>(string filePath)
        {
            string path = Application.persistentDataPath + saveFolder + filePath + filetype;
            FileStream stream = File.Open(path, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(T), XMLSurrogate.GetLoadedSurrogates());
            T data = SaveExists(filePath) ? (T)serializer.Deserialize(stream) : default;
            stream.Close();
            return data;
        }

        /// <summary> Ensures the file at the given path exists. </summary>
        /// <param name="filePath"> file path relative to <c>Application.persistentDataPath</c></param>
        public static bool SaveExists(string filePath)
        {
            string path = Application.persistentDataPath + saveFolder + filePath + filetype;
            return File.Exists(path);
        }
    }
}
