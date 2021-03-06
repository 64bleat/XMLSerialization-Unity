﻿using System.Collections.Generic;
using UnityEngine;

namespace Serialization
{
    /// <summary>
    /// PersistentParent GameObjects act as parents for serializeable objects,
    /// so that serializeable gameobjects don't have to be root objects.
    /// if a PersistentParent is destroyed, children of it cannot be reserialized.
    /// </summary>
    public class PersistentParent : MonoBehaviour
    {
        [Tooltip("A unique value to identify this PersistentParent within the scene")]
        public string persistentID;

        private static readonly Dictionary<string, GameObject> dic = new Dictionary<string, GameObject>();

        private void Awake()
        {
            if (persistentID == null || persistentID.Length == 0)
                Debug.Log($"Empty PersistentID on {gameObject.name}");
            else if(dic.ContainsKey(persistentID))
                Debug.LogError($"PersistentID duplicate on {gameObject.name}, ID: {persistentID}", gameObject);
            else
                dic.Add(persistentID, gameObject);   
        }

        private void OnDestroy()
        {
            if (persistentID != null && dic.ContainsKey(persistentID))
                dic.Remove(persistentID);
        }

        public static Transform GetPersistentParent(string parentID)
        {
            if (parentID != null && dic.TryGetValue(parentID, out GameObject go))
                return go.transform;
            else
                return null;
        }
    }
}
