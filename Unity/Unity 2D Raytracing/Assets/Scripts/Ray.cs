using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Ray : MonoBehaviour
{
    private Ray m_bounce = null;
    private RayHit m_bounceInfo = null;
    [SerializeField] private Color m_color = new Color(1,1,1);
    [SerializeField] private int m_maxDepth = 3;
    [SerializeField] private RayCollider m_origin = null;

    public void init(Color color, int maxDepth)
    {
        m_color = color;
        m_maxDepth = maxDepth;
    }

    public Vector2 position
    {
        get
        {
            return new Vector2(this.transform.position.x, this.transform.position.y);
        }
    }

    public Color color
    {
        set
        {
            m_color = value;
        }
        get
        {
            return m_color;
        }
    }

    public RayCollider origin
    {
        get
        {
            return m_origin;
        }
    }

    public Vector2 direction
    {
        get
        {
            float angle = this.transform.rotation.eulerAngles.z;
            Vector2 dir = new Vector2(Mathf.Cos(Mathf.Deg2Rad*angle), Mathf.Sin(Mathf.Deg2Rad * angle)).normalized;
            return dir;
        }
    }

    public Vector2 normal
    {
        get
        {
            Vector2 dir = direction;
            return new Vector2(-dir.y, dir.x);
        }
    }

    public Ray reflect(RayHit hit)
    {
        if (hit == null)
        {
            if (m_bounce != null) Destroy(m_bounce.gameObject);
            m_bounceInfo = null;
            return null;
        }
        else if (hit.Equals(m_bounceInfo)) return m_bounce;
        else if (m_maxDepth == 0) return null;
        else if (m_bounce != null) Destroy(m_bounce.gameObject);
        GameObject reflectRayGo = new GameObject();
        Ray reflect = reflectRayGo.AddComponent<Ray>();
        reflect.init(m_color * hit.color, m_maxDepth - 1);
        reflectRayGo.transform.position = hit.point + hit.normal.normalized*0.1f;
        reflectRayGo.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(hit.normal.y, hit.normal.x), new Vector3(0, 0, 1));
        reflectRayGo.tag = "Ray";
        m_bounce = reflect;
        m_bounceInfo = hit;
        return reflect;
    }

    public static Ray create(Vector2 start, Vector2 direction, RayCollider origin, Color color, int maxDepth = 3)
    {
        GameObject rayGo = new GameObject();
        Ray ray = rayGo.AddComponent<Ray>();
        ray.init(color, maxDepth);
        rayGo.transform.position = start;
        rayGo.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x), new Vector3(0, 0, 1));
        rayGo.tag = "Ray";
        ray.m_origin = origin;
        return ray;
    }

    public static Ray create(Vector2 start, Vector2 direction, RayCollider origin, int maxDepth = 3)
    {
        return Ray.create(start, direction, origin, new Color(1, 1, 1), maxDepth);
    }

    public void Update()
    {
        Debug.DrawRay(position, direction, m_color);
    }

    public void OnDestroy()
    {
        if (m_bounce != null) Destroy(m_bounce.gameObject);
        m_bounceInfo = null;
    }
}

[CustomEditor(typeof(Ray))]
public class RayEditor : Editor
{
    private Ray m_ray;

    public void OnSceneGUI()
    {
        m_ray = this.target as Ray;
        Handles.color = Color.red;
        Handles.DrawLine(m_ray.position, m_ray.position + m_ray.direction);
    }
}