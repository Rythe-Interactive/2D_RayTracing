using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RayCircleCollider : RayCollider
{
    [SerializeField] private float m_radius;
    [SerializeField] private readonly Vector2 m_offset;

    public void Start()
    {
        RayCaster.register(this);
    }

    public override bool collide(Ray ray, out RayHit hit)
    {
        Vector2 pos = center;
        Vector2 dist = pos - ray.position; // Distance between ray start and circle center

        float centerToRay = Vector2.Dot(dist, ray.normal.normalized);

        if (Mathf.Abs(centerToRay) > m_radius)
        {
            hit = null;
            return false;
        }

        Vector2 closestRayPoint = ray.position + ray.direction * Vector2.Dot(dist, ray.direction.normalized);
        float closestRayPointToPoi = Mathf.Sqrt(Mathf.Pow(m_radius, 2) - Mathf.Pow(centerToRay, 2));

        Vector2 poi = closestRayPoint - (closestRayPointToPoi * ray.direction.normalized);
        Vector2 rayToPoi = poi - ray.position;
        if (rayToPoi.normalized != ray.direction)
        {
            hit = null;
            return false;
        }
        Vector2 normal = (poi - center).normalized;
        hit = new RayHit(ray, poi, normal, this);
        return true;
    }

    public Vector2 center
    {
        get
        {
            return new Vector2(this.transform.position.x, this.transform.position.y) + m_offset;
        }
    }

    public float radius
    {
        get
        {
            return m_radius;
        }
    }
}

[CustomEditor(typeof(RayCircleCollider))]
public class RayCircleColliderEditor : Editor
{
    private RayCircleCollider m_collider;

    public void OnSceneGUI()
    {
        m_collider = this.target as RayCircleCollider;
        Handles.color = Color.red;
        Handles.DrawWireDisc(m_collider.center, Vector3.forward, m_collider.radius);
    }
}