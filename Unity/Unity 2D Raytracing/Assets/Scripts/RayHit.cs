using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RayHit
{
    public readonly Ray ray;
    public readonly Vector2 point;
    public readonly Vector2 normal;
    public readonly RayCollider collider;
    public readonly Color color;
    public readonly bool nullHit;

    public RayHit(Ray ray, Vector2 point, Vector2 normal, RayCollider collider, Color color)
    {
        this.ray = ray;
        this.point = point;
        this.normal = normal;
        this.collider = collider;
        this.color = color;
        this.nullHit = false;
    }

    // Sets ray hit to 'null'
    // 'null' can be detected using the RayHit.nullHit
    public RayHit(Ray ray)
    {
        this.ray = ray;
        this.point = new Vector2(0,0);
        this.normal = new Vector2(0,0);
        this.collider = null;
        this.color = new Color(0,0,0);
        this.nullHit = true;
    }

    public bool Equals(RayHit hit)
    {
        if (hit.nullHit && nullHit && hit.ray == ray) return true;
        else if (hit.nullHit != nullHit) return false;
        else if (hit.nullHit && nullHit && hit.ray != ray) return false;
        else if (hit.ray == ray && hit.point == point && hit.normal == normal && hit.collider == collider && hit.color == color) return true;
        else return false;
    }
}
