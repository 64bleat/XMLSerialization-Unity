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

        public override XMLSurrogate Serialize(object o)
        {
            Transform t = o as Transform;

            if (t)
            {
                position = t.position;
                rotation = t.rotation;
                localScale = t.localScale;
                parentPath = GetParentPath(t, ref parentID);
            }

            return this;
        }

        public override XMLSurrogate Deserialize(object o)
        {
            Transform t = o as Transform;

            if(t)
            {
                //t.parent = GetParentFor(parentPath, parentID);
                t.SetSiblingIndex(parentPath[parentPath.Length - 1]);
                t.SetPositionAndRotation(position, rotation);
                t.localScale = localScale;
            }

            return this;
        }

        public static int[] GetParentPath(Transform t, ref string persistentParent)
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

        //public static GameObject GetGameObjectAt(int[] parentPath)
        //{
        //    Transform transform = null;
        //    string deb = "", boob = "";

        //    try
        //    {
        //        if (parentPath != null && parentPath.Length > 0)
        //        {
        //            GameObject[] root = SceneManager.GetActiveScene().GetRootGameObjects();
        //            transform = root[parentPath[0]].transform;
        //            deb += transform.name + "->";

        //            foreach (var gg in root)
        //                boob += gg.name + '\n';

        //            boob += "__________________\n";

        //            for (int i = 1; transform && i < parentPath.Length; i++)
        //            {
        //                for (int j = 0; j < transform.childCount; j++)
        //                    boob += transform.GetChild(j).gameObject.name + "\n";

        //                boob += "__________________\n";

        //                transform = transform.GetChild(parentPath[i]);

        //                deb += transform.name + "->";
        //            }
        //        }
        //    }
        //    catch(UnityException e)
        //    {
        //        Debug.Log(e.ToString());
        //    }

        //    Debug.Log(deb + '\n' + boob) ;

        //    return transform.gameObject;
        //}

        //public static Transform GetPersistentParent(string parentID, GameObject[] list)
        //{
        //    if (PersistentParent.GetPersistentParent(parentID) is var pp && pp)
        //        return pp.transform;
        //    //else if (list != null && parentID != null && parentID.Length != 0)
        //    //    foreach (GameObject go in list)
        //    //        if (go.GetComponent<PersistentParent>() is var p && p && p.persistentID.Equals(parentID))
        //    //            return go.transform;
        //    //        else if (GetPersistentParent(parentID, GetChildGameObjects(go)) is var ch && ch)
        //    //            return ch;

        //    return null;
        //}

        public static Transform GetParentFor(int[] parentPath, string parentID)
        {
            GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();
            Transform parent = PersistentParent.GetPersistentParent(parentID);

            try
            {
                if (parentPath != null && parentPath.Length > 1)
                {
                    if (!parent)
                        parent = roots[parentPath[0]].transform;

                    for (int i = 1; i < parentPath.Length - 1; i++)
                        parent = parent.GetChild(parentPath[i]);
                }
            }
            catch { Debug.Log("Parenting failed on transform deserialization"); }

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
