using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RayCaster
{
    private static List<RayCollider> m_colliders;
    private static List<RayHit> m_rayHits;

    public static void register(RayCollider collider)
    {
        if (m_colliders == null) m_colliders = new List<RayCollider>();
        m_colliders.Add(collider);
    }

    public static void unRegister(RayCollider collider)
    {
        if (m_colliders == null) return;
        m_colliders.Remove(collider);
    }

    public static void render()
    {
        m_rayHits?.Clear();
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Ray").Length; ++i)
        {
            Ray ray = GameObject.FindGameObjectsWithTag("Ray")[i].GetComponent<Ray>();
            RayHit hit = collide(ray);
            if (hit != null)
            {
                if(m_rayHits == null) m_rayHits = new List<RayHit>();
                m_rayHits.Add(hit);
            }
        }
    }

    public static RayHit collide(Ray ray)
    {
        if (m_colliders == null) return null;
        RayHit hit = null;
        float dist = 0;
        for (int i = 0; i < m_colliders.Count; ++i)
        {
            if(m_colliders[i].collide(ray, out RayHit newHit))
            {
                if(hit == null || (newHit.point-ray.position).magnitude < dist)
                {
                    dist = (newHit.point - ray.position).magnitude;
                    hit = newHit;
                }
            }
        }
        ray.setBounceFromHit(hit);
        return hit;
    }
}
