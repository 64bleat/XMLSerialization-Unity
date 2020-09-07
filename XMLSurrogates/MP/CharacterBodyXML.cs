using SHK.Creatures;
using UnityEngine;

namespace Serialization
{
    [System.Serializable]
    [XMLSurrogate(typeof(CharacterBody))]
    public class CharacterBodyXML : XMLSurrogate
    {
        public Vector3 velocity;
        public float lookY;

        public override XMLSurrogate Serialize(object o)
        {
            CharacterBody c = o as CharacterBody;

            if(c)
            {
                velocity = c.Velocity;
                lookY = c.lookY;
            }

            return this;
        }

        public override XMLSurrogate Deserialize(object o)
        {
            CharacterBody c = o as CharacterBody;

            if (c)
            {
                c.Velocity = velocity;
                c.lookY = lookY;
            }

            return this;
        }
    }
}