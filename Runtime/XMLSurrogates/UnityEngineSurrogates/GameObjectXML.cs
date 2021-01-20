using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace Serialization
{
    [System.Serializable]
    [XMLSurrogate(typeof(GameObject))]
    public class GameObjectXML : XMLSurrogate
    {
        [XmlAttribute] public string name;
        [XmlAttribute] public string tag;
        [XmlAttribute] public int layer;
        [XmlAttribute] public int instanceID;
        [XmlAttribute] public bool isStatic;
        [XmlAttribute] public bool activeSelf;
        public List<XMLSurrogate> componentData;

        private GameObject instance;

        private static readonly Dictionary<string, GameObject> loadedGameObjects = new Dictionary<string, GameObject>();
        private static readonly List<Type> componentBuffer = new List<Type>();

        public GameObjectXML() 
        {
            // Necessary for serialization
        }

        public GameObjectXML(GameObject go)
        {
            Serialize(go);
        }

        public override XMLSurrogate Serialize(object o)
        {
            GameObject go = o as GameObject;

            name = go.name;
            tag = go.tag;
            layer = go.layer;
            instanceID = go.GetInstanceID();
            isStatic = go.isStatic;
            activeSelf = go.activeSelf;
            componentData = new List<XMLSurrogate>();

            foreach (Component component in go.GetComponents<Component>())
                if (InstantiateSurrogateFor(component.GetType()) is var surrogate && surrogate != null)
                    componentData.Add(surrogate.Serialize(component));

            return this;
        }

        /// <param name="_"> unused null, surrogate handles instantiation </param>
        /// <returns> a newly instantiated and deserialized GameObject </returns>
        public override XMLSurrogate Deserialize(dynamic _)
        {
            XMLSerializeableXML sInfo = null;
            TransformXML tInfo = null;

            for (int i = 0; i < componentData.Count && sInfo == null; i++)
                sInfo = componentData[i] as XMLSerializeableXML;

            for (int i = 0; i < componentData.Count && tInfo == null; i++)
                tInfo = componentData[i] as TransformXML;

            /*  1:  Finds a persistent parent in scene to deserialize onto.
             *  2:  Deserializes onto an instantiated prefab.
             *  3:  Deserializes onto a brand new, empty GameObject.   */
            if(XMLSerializeable.TryGetPersistentGameObject(sInfo.persistentID, out GameObject go))
                DeserializeGameObject(go);
            else if(LoadResource(sInfo.resourceID) is GameObject pr && pr)
                DeserializeGameObject(
                    UnityEngine.Object.Instantiate(pr, 
                        tInfo.position, tInfo.rotation, 
                        TransformXML.GetParentFor(tInfo.parentPath, tInfo.parentID)));
            else
            {
                componentBuffer.Clear();
                // Get needed Component types for serialized Components.
                foreach(XMLSurrogate componentSurrogate in componentData)
                    if(XMLSurrogate.TryGetComponentType(componentSurrogate.GetType(), out Type componentType)
                        && !componentType.Equals(typeof(Transform)))
                        componentBuffer.Add(componentType);

                DeserializeGameObject(new GameObject(name, componentBuffer.ToArray()));
            }

            return this;
        }

        /// <summary>
        /// The desired scene must be loaded before gameobjects can be deserialized.
        /// Deserialization is separated so it may be called on <c>SceneManager.sceneLoaded</c>
        /// if it is not already loaded.
        /// </summary>
        /// <param name="go"></param>
        public void DeserializeGameObject(GameObject go)
        {
            if (go)
            {

                go.name = name;
                go.tag = tag;
                go.layer = layer;
                go.isStatic = isStatic;

                go.SetActive(false);

                Component[] components = go.GetComponents<Component>();

                foreach (Component c in components)
                    if(TryTakeSurrogate(componentData, GetSurrogateTypeFor(c.GetType()), out XMLSurrogate surrogate))
                        surrogate.Deserialize(c);

                go.SetActive(activeSelf);
            }
        }

        /// <summary>
        /// Like Resources.Load, but stores the result in a dictionary
        /// so it may be retrieved without having to reload it.
        /// </summary>
        private static GameObject LoadResource(string key)
        {
            GameObject resource = null;

            if(key != null && !loadedGameObjects.TryGetValue(key, out resource))
            {
                resource = Resources.Load(key) as GameObject;

                if (resource)
                    loadedGameObjects.Add(key, resource);
            }

            return resource;
        }

        /// <summary>
        /// Clears LoadResource memory
        /// </summary>
        public static void UnloadResources()
        {
            loadedGameObjects.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool TryTakeSurrogate(List<XMLSurrogate> list, Type type, out XMLSurrogate surrogate)
        {
            for(int i = 0; i < list.Count; i++)
                if(type?.Equals(list[i].GetType()) == true)
                {
                    surrogate = list[i];
                    list.RemoveAt(i);
                    return true;
                }

            surrogate = null;

            return false;
        }
    }
}
