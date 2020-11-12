using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Ray
{
    private bool m_hasBounce;
    private Ray m_bounce;
    private RayHit m_bounceInfo;
    private Vector2 m_position;
    private Vector2 m_direction;
    private Color m_color;
    private int m_maxDepth;
    private RayCollider m_origin;
    private float m_intensity;

    private const int stdDepth = 2;
    private Ray m_parent = null;

    static List<Ray> m_recycledRays;

    public static Ray requestRay(Vector2 position, Vector2 direction, RayCollider origin, float intensity, Color color, int maxDepth = stdDepth)
    {
        Ray ray;
        if (m_recycledRays != null && m_recycledRays.Count != 0)
        {
            int last = m_recycledRays.Count - 1;
            ray = m_recycledRays[last];
            m_recycledRays.RemoveAt(last);
            ray.reUse(position.x, position.y, direction.x, direction.y, origin, intensity, color.r, color.g, color.b, color.a, maxDepth);
        }
        else ray = new Ray(position, direction, origin, intensity, color, maxDepth);

#if (UNITY_EDITOR)
        RayVisualizer.instance.register(ray);
#endif
        return ray; 
    }

    public static Ray requestRay(Vector2 position, Vector2 direction, float intensity, Color color, int maxDepth = stdDepth)
    {
        return requestRay(position, direction, null, intensity, color, maxDepth);
    }

    public static Ray requestRay(Vector2 position, Vector2 direction, RayCollider origin, float intensity, int maxDepth = stdDepth)
    {
        return requestRay(position, direction, origin, intensity, new Color(1, 1, 1), maxDepth);
    }

    public static void recycleRay(Ray ray)
    {
        if (m_recycledRays == null) m_recycledRays = new List<Ray>();
#if (UNITY_EDITOR)
        RayVisualizer.instance.unRegister(ray);
#endif
        m_recycledRays.Add(ray);
    }

    public static int recycledRayCount()
    {
        if (m_recycledRays == null) return 0;
        return m_recycledRays.Count;
    }

    private Ray(Vector2 position, Vector2 direction, RayCollider origin, float intensity, Color color, int maxDepth = stdDepth)
    {
        m_position = position;
        m_direction = direction;
        m_color = color;
        m_maxDepth = maxDepth;
        m_origin = origin;
        m_intensity = intensity;

        m_hasBounce = false;
        m_bounce = null;
        m_bounceInfo = new RayHit(this);
    }

    private Ray(Vector2 position, Vector2 direction, float intensity, Color color, int maxDepth = stdDepth) : 
        this(position, direction, null, intensity, color, maxDepth) { }

    private Ray(Vector2 position, Vector2 direction, RayCollider origin, float intensity, int maxDepth = stdDepth) :
        this(position, direction, origin, intensity, new Color(1, 1, 1), maxDepth) { }

    ~Ray()
    {
#if (UNITY_EDITOR)
        RayVisualizer.instance.unRegister(this);
#endif
    }

    #region getters

    public Vector2 position
    {
        get
        {
            return m_position;
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
            return m_direction;
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

    public float intensity
    {
        get
        {
            return m_intensity;
        }
    }

    #endregion

    public void resetReflect()
    {
        m_hasBounce = false;
        if (m_bounce != null)
        {
            m_bounce.reUse(0, 0, 0, 0, null, 0, 0, 0, 0, 0);
            m_bounce.resetReflect();
        }
    }

    public Ray reflect(RayHit hit)
    {
        if (hit.nullHit || hit.fromInsideShape || m_color * hit.color == Color.black)
        {
            resetReflect();
            return null;
        }
        //else if (m_hasBounce && hit.Equals(m_bounceInfo))
        //{
        //    return m_bounce;
        //}
        else if (m_maxDepth == 0) return null;
        
        //Reflect happens
        Vector2 reflectDir = Vector2.Reflect(m_direction, hit.normal).normalized;
        if (m_bounce == null)
        {
            float intensity = hit.ray.m_intensity - Mathf.Abs((hit.point - hit.ray.position).magnitude);
            if(intensity <= 0)
            {
                // The new ray will not have enough intensity to transfer
                resetReflect();
                return null;
            }
            m_bounce = requestRay(hit.point, reflectDir, hit.collider, intensity, m_color * hit.color, m_maxDepth - 1);
            m_bounceInfo = hit;

        }
        else
        {
            float intensity = hit.ray.m_intensity - Mathf.Abs((hit.point - hit.ray.position).magnitude);
            if (intensity <= 0)
            {
                // The new ray will not have enough intensity to transfer
                resetReflect();
                return null;
            }
            Color cl = m_color * hit.color;
            m_bounce.reUse(hit.point.x, hit.point.y, reflectDir.x, reflectDir.y, hit.collider, intensity, cl.r, cl.g, cl.b, cl.a, m_maxDepth - 1);
        }
        m_hasBounce = true;
        m_bounce.m_parent = this;
        return m_bounce;
    }

    public Ray parent
    {
        get
        {
            return m_parent;
        }
    }

    public Ray getBounce()
    {
        return m_bounce;
    }

    public bool hasBounce()
    {
        return m_hasBounce;
    }

    public List<Ray> getBounces()
    {
        List<Ray> rays = new List<Ray>();
        if (m_hasBounce)
        {
            rays.Add(m_bounce);
            List<Ray> childBounces = m_bounce.getBounces();
            if(childBounces != null)
            {
                rays.AddRange(childBounces);
            }
        }
        return rays;
    }


    public void setColor(float r, float g, float b, float a = 1.0f)
    {
        m_color.r = r;
        m_color.g = g;
        m_color.b = b;
        m_color.a = a;
    }

    public void setPosition(float x, float y)
    {
        m_position.Set(x, y);
        m_hasBounce = false;
    }

    public void setDirection(float x, float y)
    {
        m_direction.Set(x, y);
        m_hasBounce = false;
    }

    public void reset()
    {
        reUse(0, 0, 0, 0, null, 0, 0, 0, 0, 0);
    }

    public void reUse(float x, float y, float dirX, float dirY, RayCollider origin, float intensity, float r = 1, float g = 1, float b = 1, float a = 1, int maxDepth = stdDepth)
    {
        m_position.Set(x, y);
        m_direction.Set(dirX, dirY);
        setColor(r, g, b, a);
        m_maxDepth = maxDepth;
        m_origin = origin;
        m_intensity = intensity;

        m_hasBounce = false;
    }
}