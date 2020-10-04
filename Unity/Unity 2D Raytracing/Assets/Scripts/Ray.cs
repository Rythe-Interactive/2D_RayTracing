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

    public Ray(Vector2 position, Vector2 direction, RayCollider origin, Color color, int maxDepth = 1)
    {
        m_position = position;
        m_direction = direction;
        m_color = color;
        m_maxDepth = maxDepth;
        m_origin = origin;

        m_hasBounce = false;
        m_bounce = null;
        m_bounceInfo = new RayHit(this);
        RayVisualizer.instance.register(this);
    }

    public Ray(Vector2 position, Vector2 direction, Color color, int maxDepth = 1) : 
        this(position, direction, null, color, maxDepth) { }

    public Ray(Vector2 position, Vector2 direction, RayCollider origin, int maxDepth = 1) :
        this(position, direction, origin, new Color(1, 1, 1), maxDepth) { }

    ~Ray()
    {
        RayVisualizer.instance.unRegister(this);
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

    #endregion

    public void resetReflect()
    {
        m_hasBounce = false;
        if (m_bounce != null)
        {
            m_bounce.setColor(0, 0, 0, 0);
        }
    }

    public Ray reflect(RayHit hit)
    {
        if (hit.nullHit)
        {
            resetReflect();
            return null;
        }
        else if (m_hasBounce && hit.Equals(m_bounceInfo))
        {
            return m_bounce;
        }
        else if (m_maxDepth == 0) return null;
        //Reflect happens

        Vector2 reflectDir = Vector2.Reflect(m_direction, hit.normal).normalized;
        m_hasBounce = true;
        if (m_bounce == null)
        {
            m_bounce = new Ray(hit.point, reflectDir, m_color * hit.color, m_maxDepth - 1);
            m_bounceInfo = hit;
        }
        else
        {
            Color cl = m_color * hit.color;
            m_bounce.reUse(hit.point.x, hit.point.y, reflectDir.x, reflectDir.y, cl.r, cl.g, cl.b, cl.a, m_maxDepth - 1);
        }
        return m_bounce;
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

    public void reset()
    {
        reUse(0, 0, 0, 0, null, 0, 0, 0, 0, 0);
    }

    public void reUse(float x, float y, float dirX, float dirY, float r = 1, float g = 1, float b = 1, float a = 1, int maxDepth = 1)
    {
        reUse(x, y, dirX, dirY, null, r, g, b, a, maxDepth);
    }

    public void reUse(float x, float y, float dirX, float dirY, RayCollider origin, float r = 1, float g = 1, float b = 1, float a = 1, int maxDepth = 1)
    {
        m_position.Set(x, y);
        m_direction.Set(dirX, dirY);
        setColor(r, g, b, a);
        m_maxDepth = maxDepth;
        m_origin = origin;

        m_hasBounce = false;
    }
}