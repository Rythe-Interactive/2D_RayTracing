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
            List<RayHit> rays = collide(ray);
            if (m_rayHits == null) m_rayHits = new List<RayHit>();
        }
    }

    public static List<RayHit> collide(Ray ray)
    {
        if (m_colliders == null) return null;
        List<RayHit> hits = new List<RayHit>();
        for (int i = 0; i < m_colliders.Count; ++i)
        {
            if(m_colliders[i].collide(ray, out RayHit hit))
            {
                hits.Add(hit);
                ray.setBounceFromHit(hit);
            }
        }
        return hits;
    }
}
