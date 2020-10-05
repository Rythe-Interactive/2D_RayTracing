using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RayCollider : MonoBehaviour
{
    public abstract bool collide(Ray ray, out RayHit hit);
    public abstract void registerHit(RayHit hit);
    public abstract void clearHits();
    public abstract void applyHits();
}
