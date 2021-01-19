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
            XmlSerializer serializer = new XmlSerializer(typeof(T), XMLSurrogate.GetLoadedSurrogates());
            Stream stream = File.Create(path);

            serializer.Serialize(stream, saveData);
            stream.Close();
        }

        public static string SaveToString<T>(T data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), XMLSurrogate.GetLoadedSurrogates());
            StringWriter writer = new StringWriter();

            serializer.Serialize(writer, data);

            return writer.ToString();
        }

        /// <summary> Saves an instance of a type and its heirarchy into an xml file </summary>
        /// <typeparam name="T"> specifies the type being de-serialized </typeparam>
        /// <param name="filePath"> file path relative to <c>Application.persistentDataPath</c></param>
        public static T Load<T>(string filePath)
        {
            string path = Application.persistentDataPath + saveFolder + filePath + filetype;
            T data;

            if (SaveExists(filePath))
            {
                FileStream stream = File.Open(path, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(T), XMLSurrogate.GetLoadedSurrogates());

                data = (T)serializer.Deserialize(stream);
                stream.Close();
            }
            else
                data = default;

            
            return data;
        }

        public static bool TryLoad<T>(string filePath, out T data)
        {
            string path = Application.persistentDataPath + saveFolder + filePath + filetype;
            bool exists = SaveExists(filePath);

            if (exists)
            {
                FileStream stream = File.Open(path, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(T), XMLSurrogate.GetLoadedSurrogates());

                data = (T)serializer.Deserialize(stream);
                stream.Close();
            }
            else
                data = default;


            return exists;
        }

        /// <summary> Ensures the file at the given path exists. </summary>
        /// <param name="fileName"> file path relative to <c>Application.persistentDataPath</c></param>
        public static bool SaveExists(string fileName)
        {
            string path = Application.persistentDataPath + saveFolder + fileName + filetype;
            return File.Exists(path);
        }
    }
}
