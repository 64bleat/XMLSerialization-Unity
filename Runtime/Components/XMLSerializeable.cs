using System.Collections.Generic;
using UnityEngine;

namespace Serialization
{
    /// <summary> Marks a GameObject as serializeable </summary>
    public class XMLSerializeable : MonoBehaviour
    {
        [Tooltip("The resource to instantiate upon deserialization")]
        public string resourceID;
        [Tooltip("Set this to a unique id if this GameObject isn't meant to be destroyed on deserialization.")]
        public string persistentID;

        private static readonly Dictionary<string, GameObject> dic = new Dictionary<string, GameObject>();

        private void Awake()
        {
            if (persistentID != null && persistentID.Length != 0 && !dic.ContainsKey(persistentID))
                dic.Add(persistentID, gameObject);
        }

        private void OnDestroy()
        {
            if (persistentID != null && dic.ContainsKey(persistentID))
                dic.Remove(persistentID);
        }

        public static GameObject GetPersistentGameObject(string persistentID)
        {
            if (persistentID != null && dic.TryGetValue(persistentID, out GameObject go))
                return go;
            else
                return null;
        }

        public static bool TryGetPersistentGameObject(string guid, out GameObject gameObject)
        {
            return dic.TryGetValue(guid ?? "", out gameObject);
        }
    }
}
