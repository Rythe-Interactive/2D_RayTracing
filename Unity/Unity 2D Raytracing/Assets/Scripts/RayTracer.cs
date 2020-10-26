using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventRayTracedLight : UnityEvent<RayTracedLight>
{

}

public class RayTracer : MonoBehaviour
{
    [SerializeField] int m_colliderCount = 0;
    [SerializeField] int m_rayCount = 0;
    [SerializeField] int m_lightCount = 0;
    private List<RayCollider> m_colliders;
    private List<RayHit> m_rayHits;
    private List<Ray> m_rays;
    private List<RayTracedLight> m_lights;
    private UnityEventRayTracedLight m_onLightAdd;
    private UnityEventRayTracedLight m_onLightRemove;

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
        m_onLightAdd?.Invoke(light);
    }

    public void unRegister(RayTracedLight light)
    {
        m_lights.Remove(light);
        m_onLightRemove?.Invoke(light);
        --m_lightCount;
    }

    public void register(RayCollider collider)
    {
        if (m_colliders == null)
        {
            m_colliders = new List<RayCollider>();
            m_onLightAdd = new UnityEventRayTracedLight();
            m_onLightRemove = new UnityEventRayTracedLight();
        }
        for(int i = 0; i < m_colliders.Count; ++i)
        {
            collider.onColliderAdd(m_colliders[i]);
            m_colliders[i].onColliderAdd(collider);
        }
        m_colliders.Add(collider);
        ++m_colliderCount;
        if (m_lights != null)
        {
            for (int i = 0; i < m_lights.Count; ++i)
            {
                collider.onLightAdd(m_lights[i]);
            }
        }
    }

    public void unRegister(RayCollider collider)
    {
        if (m_colliders == null) return;
        m_colliders.Remove(collider);
        for (int i = 0; i < m_colliders.Count; ++i)
        {
            collider.onColliderRemove(m_colliders[i]);
            m_colliders[i].onColliderRemove(collider);
        }
        --m_colliderCount;
    }

    public void callBackOnLightAdd(UnityAction<RayTracedLight> action)
    {
        m_onLightAdd?.AddListener(action);
    }

    public void callBackOnLightRemove(UnityAction<RayTracedLight> action)
    {
        m_onLightAdd?.RemoveListener(action);
    }

    public void Update()
    {
        if (m_rays == null) return;
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

        if (m_colliders != null)
        {
            for (int i = 0; i < m_colliders.Count; ++i)
            {
                m_colliders[i].applyHits();
            }
        }
    }

    private void updateSerializeFields()
    {
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
