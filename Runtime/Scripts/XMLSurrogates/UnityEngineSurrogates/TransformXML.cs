using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Serialization
{
    [System.Serializable]
    [XMLSurrogate(typeof(Transform))]
    public class TransformXML : XMLSurrogate
    {
        public string parentID;
        public Vector3 position;
        public Vector3 localScale;
        public Quaternion rotation;
        public int[] parentPath;

        private Transform transform;
        private int siblingIndex;

        public override XMLSurrogate Serialize(dynamic o)
        {
            if (o is Transform t)
            {
                position = t.position;
                rotation = t.rotation;
                localScale = t.localScale;
                parentPath = GetParentPath(t, ref parentID);
            }

            return this;
        }

        public override XMLSurrogate Deserialize(dynamic o)
        {
            if (o is Transform t)
            {
                //t.SetSiblingIndex(parentPath[parentPath.Length - 1]);
                t.SetPositionAndRotation(position, rotation);
                t.localScale = localScale;

                if (parentPath != null && parentPath.Length > 0)
                    siblingIndex = parentPath[parentPath.Length - 1];

                transform = t;

                SceneXML.OnSceneSerialized += OnSceneSerializationComplete;
            }

            return this;
        }

        private void OnSceneSerializationComplete()
        {
            transform.SetSiblingIndex(siblingIndex);
        }

        private static int[] GetParentPath(Transform t, ref string persistentParent)
        {
            Stack<int> parentPath = new Stack<int>();

            do
            {
                parentPath.Push(t.GetSiblingIndex());

                if (t.gameObject.GetComponent<PersistentParent>() is var pp && pp)
                {
                    persistentParent = pp.persistentID;
                    break;
                }       
            }
            while (t = t.parent);

            return parentPath.ToArray();
        }

        public static Transform GetParentFor(int[] parentPath, string parentID)
        {
            Transform parent = PersistentParent.GetPersistentParent(parentID);

            try
            {
                if (parentPath != null && parentPath.Length > 1)
                {
                    if (!parent)
                    {
                        GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();

                        parent = roots[parentPath[0]].transform;
                    }

                    for (int i = 1; i < parentPath.Length - 1; i++)
                        parent = parent.GetChild(parentPath[i]);
                }
            }
            catch 
            {
                Debug.Log("Parenting failed on transform deserialization"); 
            }

            return parent;
            }



        public static Transform[] GetChildren(Transform t)
        {
            Transform[] children = null;

            if(t)
            {
                children = new Transform[t.childCount];

                for (int i = 0; i < children.Length; i++)
                    children[i] = t.GetChild(i);
            }

            return children;
        }

        public static GameObject[] GetChildGameObjects(GameObject go)
        {
            GameObject[] children = null;

            if (go)
            {
                children = new GameObject[go.transform.childCount];

                for (int i = 0; i < children.Length; i++)
                    children[i] = go.transform.GetChild(i).gameObject;
            }

            return children;
        }
    }
}
