using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RayCollider : MonoBehaviour
{
    public abstract bool collide(Ray ray, out RayHit hit);
    public abstract bool pointOnSurface(Vector2 point);

    public bool pointOnSurface(float x, float y) { return pointOnSurface(new Vector2(x, y)); }
    public abstract void setRay(Ray ray, float x, float y);
}
