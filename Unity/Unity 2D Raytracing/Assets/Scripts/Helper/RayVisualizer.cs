using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayVisualizer : MonoBehaviour
{
    private List<Ray> m_rays;
    public static RayVisualizer instance;
    [SerializeField] int m_rayCount;
    [SerializeField] bool m_drawFirstIteration = true;

    // Start is called before the first frame update
    void Awake()
    {
        m_rays = new List<Ray>();
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    public void FixedUpdate()
    {
        if (m_drawFirstIteration)
        {
            for (int i = 0; i < m_rays.Count; ++i)
            {
                if (m_rays[i].color.a >= 0)
                {
                    Vector2 direction;
                    if (m_rays[i].hasBounce())
                    {
                        direction = m_rays[i].getBounce().position - m_rays[i].position;
                    }
                    else direction = m_rays[i].direction.normalized * m_rays[i].intensity;
                    Debug.DrawRay(m_rays[i].position, direction, m_rays[i].color, 0);
                }
            }
        }
        else
        {
            for (int i = 0; i < m_rays.Count; ++i)
            {
                if (!m_rays[i].hasBounce()) continue;
                Ray ray = m_rays[i].getBounce();
                if (ray.color.a >= 0)
                {
                    Vector2 direction;
                    if (ray.hasBounce())
                    {
                        direction = ray.getBounce().position - ray.position;
                    }
                    else direction = ray.direction.normalized * ray.intensity;
                    Debug.DrawRay(ray.position, direction, ray.color, 0);
                }
            }
        }
        m_rayCount = m_rays.Count;
    }

    public void register(Ray ray)
    {
        m_rays.Add(ray);
    }

    public void unRegister(Ray ray)
    {
        List<Ray> children = ray.getBounces();
        for(int i = 0; i < children.Count; ++i)
        {
            m_rays.Remove(children[i]);
        }
        m_rays.Remove(ray);
    }

    public void clear()
    {
        m_rays.Clear();
    }
}
