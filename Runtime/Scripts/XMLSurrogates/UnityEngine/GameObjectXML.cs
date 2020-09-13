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
        public XMLSurrogate[] componentData;

        private static readonly Dictionary<string, GameObject> loadedGameObjects = new Dictionary<string, GameObject>();

        public GameObjectXML() { }
        public GameObjectXML(GameObject go)
        {
            Serialize(go);
        }

        public override XMLSurrogate Serialize(object o)
        {
            GameObject go = o as GameObject;
            List<XMLSurrogate> serializedComponents = new List<XMLSurrogate>();

            // Serialize GameObject Components
            foreach(Component component in go.GetComponents<Component>())
                if (InstantiateSurrogateFor(component.GetType()) is var surrogate && surrogate != null)
                    serializedComponents.Add(surrogate.Serialize(component));

            componentData = serializedComponents.ToArray();
            name = go.name;
            tag = go.tag;
            layer = go.layer;
            instanceID = go.GetInstanceID();
            isStatic = go.isStatic;
            activeSelf = go.activeSelf;

            return this;
        }

        /// <summary>
        /// Unlike other XMLSurrogates, GameObjectXML handles instancing
        /// rather than having an instance handed to it for deserialization.
        /// </summary>
        /// <param name="d"> unused </param>
        /// <returns> this </returns>
        public override XMLSurrogate Deserialize(object d)
        {
            XMLSerializeableXML sInfo = null;
            TransformXML tInfo = null;

            for (int i = 0; i < componentData.Length && sInfo == null; i++)
                sInfo = componentData[i] as XMLSerializeableXML;

            for (int i = 0; i < componentData.Length && tInfo == null; i++)
                tInfo = componentData[i] as TransformXML;

            /*  1:  Finds a persistent parent in scene to deserialize onto.
             *  2:  Deserializes onto an instantiated prefab.
             *  3:  Deserializes onto a brand new, empty GameObject.    */
            if(XMLSerializeable.GetPersistentGameObject(sInfo.persistentID) is GameObject pgo && pgo)  
                DeserializeGameObject(pgo);
            else if(LoadResource(sInfo.resourceID) is GameObject pr && pr)
                DeserializeGameObject(
                    UnityEngine.Object.Instantiate(pr, 
                        tInfo.position, tInfo.rotation, 
                        TransformXML.GetParentFor(tInfo.parentPath, tInfo.parentID)));
            else
            {
                List<Type> componentsToAdd = new List<Type>();

                // Get needed Component types for serialized Components.
                foreach(XMLSurrogate componentSurrogate in componentData)
                    if (Attribute.GetCustomAttribute(componentSurrogate.GetType(), typeof(XMLSurrogateAttribute)) is XMLSurrogateAttribute attribute 
                        && attribute?.componentType?.Equals(typeof(Transform)) == false)
                        componentsToAdd.Add(attribute.componentType);

                DeserializeGameObject(new GameObject(name, componentsToAdd.ToArray()));
            }

            return this;
        }

        public void DeserializeGameObject(GameObject go)
        {
            if (go)
            {
                Component[] components = go.GetComponents<Component>();
                List<XMLSurrogate> serializedComponents = new List<XMLSurrogate>(componentData);

                go.name = name;
                go.tag = tag;
                go.layer = layer;
                go.isStatic = isStatic;
                go.SetActive(activeSelf);

                foreach (Component c in components)
                {
                    Type surrogateType = GetSurrogateTypeFor(c.GetType());
                    XMLSurrogate surrogate = PullSurrogate(serializedComponents, surrogateType);

                    surrogate?.Deserialize(c);
                }
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


        private static XMLSurrogate PullSurrogate(List<XMLSurrogate> list, Type type)
        {
            for(int i = 0; i < list.Count; i++)
                if(type?.Equals(list[i].GetType()) == true)
                {
                    XMLSurrogate pick = list[i];
                    list.RemoveAt(i);
                    return pick;
                }

            return null;
        }
    }
}
