using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public struct LineSegment
{
    private Vector2 m_start;
    private Vector2 m_end;

    public LineSegment(Vector2 start, Vector2 end)
    {
        m_start = start;
        m_end = end;
    }

    public bool collideWithRay(Ray ray, out Vector2 poi, out float toi)
    {
        poi = new Vector2(0, 0);
        Vector2 startToStart = m_start - ray.position;
        Vector2 lineDir = m_end - m_start;
        float cross = FibonacciCross(ray.direction, lineDir);

        // calculate time of impact of ray onto line
        // Time of impact sould be 0-1
        float u = FibonacciCross(startToStart, ray.direction) / cross;
        // Time of impact on the ray
        // Because a ray is infinite it should be > 0
        float t = FibonacciCross(startToStart, lineDir) / cross;

        if (u > 0 && u < 1 && t > 0)
        {
            // Collision
            poi = ray.position + ray.direction * t;
            toi = t;
            return true;
        }
        else
        {
            toi = 0;
            return false;
        }
    }

    private float FibonacciCross(Vector2 x, Vector2 y)
    {
        return (x.x * y.y) - (x.y * y.x);
    }

    public Vector2 start
    {
        get
        {
            return m_start;
        }
    }
    public Vector2 end
    {
        get
        {
            return m_end;
        }
    }

    public Vector2 direction
    {
        get
        {
            return (m_end - m_start).normalized;
        }
    }

    public Vector2 normal()
    {
        Vector2 dir = direction;
        return new Vector2(-dir.y, dir.x);
    }
}
