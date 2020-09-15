using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LightRay
{
    public readonly Vector2 position;
    public readonly Vector2 direction;
    private List<LightRay> m_bounces;
    private int m_maxDepth;

    public LightRay(Vector2 pos, Vector2 dir, int maxDepth = 3)
    {
        position = pos;
        direction = dir;
        m_bounces = null;
        m_maxDepth = maxDepth;
    }

    public RaycastHit2D cast(bool debugDraw = false)
    {
        m_bounces = new List<LightRay>();
        RaycastHit2D hit = Physics2D.Raycast(position, direction);
        if(hit.collider != null && m_maxDepth != 0)
        {
            Debug.Log("hit!");
            Vector2 reflect = Vector2.Reflect(direction, hit.normal).normalized;
            LightRay bounce = new LightRay(hit.point+Vector2.Scale(reflect, new Vector2(0.1f, 0.1f)), Vector3.Reflect(direction, hit.normal), m_maxDepth - 1);
            if(debugDraw) Debug.DrawRay(hit.point, Vector2.Scale(direction, new Vector2(-1, -1)), new Color(255, 0, 255));
            bounce.cast(debugDraw);
            m_bounces.Add(bounce);
        }
        if (debugDraw)
        {
            Debug.DrawRay(position, direction, new Color(255, 0, 0));
        }
        return hit;
    }
}
