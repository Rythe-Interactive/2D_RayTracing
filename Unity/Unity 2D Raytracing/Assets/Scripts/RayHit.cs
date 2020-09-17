using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayHit
{
    public readonly Ray ray;
    public readonly Vector2 point;
    public readonly Vector2 normal;
    public readonly RayCollider collider;
    public readonly Color color;

    public RayHit(Ray ray, Vector2 point, Vector2 normal, RayCollider collider, Color color)
    {
        this.ray = ray;
        this.point = point;
        this.normal = normal;
        this.collider = collider;
        this.color = color;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is RayHit) || obj == null) return false;
        RayHit other = obj as RayHit;
        return (other.ray == ray && other.point == point && other.normal == normal && other.collider == collider);
    }

}
