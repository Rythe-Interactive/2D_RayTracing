using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Ray : MonoBehaviour
{
    private Ray m_bounce = null;
    private RayHit m_bounceInfo = null;

    public Vector2 position
    {
        get
        {
            return new Vector2(this.transform.position.x, this.transform.position.y);
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

    public Ray setBounceFromHit(RayHit hit)
    {
        if (hit == null)
        {
            if(m_bounce != null) Destroy(m_bounce.gameObject);
            m_bounceInfo = null;
            return null;
        }
        else if (hit.Equals(m_bounceInfo)) return m_bounce;
        else if(m_bounce != null) Destroy(m_bounce.gameObject);
        GameObject reflectRayGo = new GameObject();
        Ray reflect = reflectRayGo.AddComponent<Ray>();
        reflectRayGo.transform.position = hit.point + hit.normal.normalized*0.1f;
        reflectRayGo.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(hit.normal.y, hit.normal.x), new Vector3(0, 0, 1));
        reflectRayGo.tag = "Ray";
        m_bounce = reflect;
        m_bounceInfo = hit;
        return reflect;
    }

    public void Update()
    {
        Debug.DrawRay(position, direction, Color.red);
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