using SHK.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Serialization
{
    [System.Serializable]
    [XMLSurrogate(typeof(Rigidbody))]
    public class RigidbodyXML : XMLSurrogate
    {
        public float mass;
        public float drag;
        public float angularDrag;
        public bool useGravity;
        public bool isKinematic;
        public Vector3 velocity;
        public Vector3 angularVelocity;
        public Vector3 position;
        public Quaternion rotation;

        public override XMLSurrogate Serialize(object o)
        {
            Rigidbody r = o as Rigidbody;

            if(r)
            {
                mass = r.mass;
                drag = r.drag;
                angularDrag = r.angularDrag;
                useGravity = r.useGravity || r.gameObject.GetComponent<IGravityUser>() != null;
                isKinematic = r.isKinematic;
                velocity = r.velocity;
                angularVelocity = r.angularVelocity;
                position = r.position;
                rotation = r.rotation;
            }

            return this;
        }

        public override XMLSurrogate Deserialize(object o)
        {
            Rigidbody r = o as Rigidbody;

            if(r)
            {
                
                r.mass = mass;
                r.drag = drag;
                r.angularDrag = angularDrag;
                r.useGravity = useGravity;
                r.isKinematic = isKinematic;
                r.velocity = velocity;
                r.angularVelocity = angularVelocity;
                r.position = position;
                r.rotation = rotation;
            }

            return this;
        }
    }
}
