using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RayTracer : MonoBehaviour
{
    [SerializeField] int m_colliderCount = 0;
    [SerializeField] int m_rayCount = 0;
    [SerializeField] int m_lightCount = 0;
    private List<RayCollider> m_colliders;
    private List<RayHit> m_rayHits;
    private List<Ray> m_rays;
    private List<RayTracedLight> m_lights;

    private bool m_reTrace = true;

    public void Awake()
    {
    }

    public void register(Ray ray)
    {
        if(m_rays == null) m_rays = new List<Ray>();
        m_rays.Add(ray);
        ++m_rayCount;
    }

    public void unRegister(Ray ray)
    {
        m_rays.Remove(ray);
        --m_rayCount;
    }

    public void register(RayTracedLight light)
    {
        if(m_lights == null)
        {
            m_lights = new List<RayTracedLight>();
        }
        m_lights.Add(light);
        ++m_lightCount;
        light.callBackOnChange(onLightChange);
    }

    public void unRegister(RayTracedLight light)
    {
        light.removeCallBackOnChange(onLightChange);
        m_lights.Remove(light);
        --m_lightCount;
    }

    public void register(RayCollider collider)
    {
        if (m_colliders == null)
        {
            m_colliders = new List<RayCollider>();
        }
        collider.callBackOnChange(onColliderChange);
        m_colliders.Add(collider);
        ++m_colliderCount;
    }

    public void unRegister(RayCollider collider)
    {
        if (m_colliders == null) return;
        m_colliders.Remove(collider);
        collider.removeCallBackOnChange(onColliderChange);
        --m_colliderCount;
    }

    public void Update()
    {
        if (m_rays == null) return;
        if (!m_reTrace) return;
        m_reTrace = false;
        m_rayHits?.Clear();
        if (m_colliders != null)
        {
            for (int i = 0; i < m_colliders.Count; ++i)
            {
                m_colliders[i].clearHits();
            }
        }

        for (int r = 0; r < m_rays.Count; ++r)
        {
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

        if (m_colliders != null)
        {
            for (int i = 0; i < m_colliders.Count; ++i)
            {
                m_colliders[i].applyHits();
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

    public void onLightChange(RayTracedLight light)
    {
        m_reTrace = true;
    }

    public void onColliderChange(RayCollider collider)
    {
        m_reTrace = true;
    }
}
