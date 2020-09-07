using SHK.Projectiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Serialization
{
    [XMLSurrogate(typeof(Projectile))]
    public class ProjectileXML : XMLSurrogate
    {
        public Vector3 velocity;

        public override XMLSurrogate Serialize(object o)
        {
            if(o is Projectile p && p)
            {
                velocity = p.Velocity;
            }

            return this;
        }

        public override XMLSurrogate Deserialize(object o)
        {
            if (o is Projectile p && p)
            {
                p.Velocity = velocity;
            }

            return this;
        }
    }
}
