using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class CollisionInfo
{
    public ParticleSystem particleCollision;
    public Collider2D collider2d;

    public CollisionInfo(Collider2D collider2d, ParticleSystem particleCollision)
    {
        this.collider2d = collider2d;
        this.particleCollision = particleCollision;
    }
}
