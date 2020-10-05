using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTracer : MonoBehaviour
{
    private List<RayCollider> m_colliders;
    private List<RayHit> m_rayHits;
    private List<Ray> m_rays;

    public void register(Ray ray)
    {
        if(m_rays == null) m_rays = new List<Ray>();
        m_rays.Add(ray);
    }

    public void unRegister(Ray ray)
    {
        m_rays.Remove(ray);
    }

    public void register(RayCollider collider)
    {
        if (m_colliders == null) m_colliders = new List<RayCollider>();
        m_colliders.Add(collider);
    }

    public void unRegister(RayCollider collider)
    {
        if (m_colliders == null) return;
        m_colliders.Remove(collider);
    }

    public void Update()
    {
        m_rayHits?.Clear();
        for(int i = 0; i < m_colliders.Count; ++i)
        {
            m_colliders[i].clearHits();
        }

        for (int r = 0; r < m_rays.Count; ++r)
        {
            bool hasBounce = true;
            List<Ray> rays = m_rays[r].getBounces();
            rays.Insert(0, m_rays[r]);
            for (int i = 0; i < rays.Count; ++i)
            {
                RayHit hit = collide(rays[i]);
                if (!hit.nullHit)
                {
                    if(m_rayHits == null) m_rayHits = new List<RayHit>();
                    m_rayHits.Add(hit);
                    hit.collider.registerHit(hit);
                }
                else
                {
                }

                if(!rays[i].hasBounce())
                {
                    rays[i].resetReflect();
                    break;
                }
            }
        }
    }

    public RayHit collide(Ray ray)
    {
        if (m_colliders == null) return new RayHit(ray);
        RayHit hit = new RayHit(ray);
        float dist = 0;
        for (int i = 0; i < m_colliders.Count; ++i)
        {
            if(m_colliders[i].collide(ray, out RayHit newHit))
            {
                if(hit.nullHit || (newHit.point-ray.position).magnitude < dist)
                {
                    dist = (newHit.point - ray.position).magnitude;
                    hit = newHit;
                }
            }
        }
        Ray reflect = ray.reflect(hit);
        return hit;
    }
}
