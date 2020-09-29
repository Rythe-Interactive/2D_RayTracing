using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTracer : MonoBehaviour
{
    private List<RayCollider> m_colliders;
    private List<RayHit> m_rayHits;
    private Ray[,] m_rays;
    public int renderLines = 87;
    private int m_renderLinesCycle;
    private static RayTracer m_instance;
    public float resolution = 1.0f;
    private Vector2Int m_rayCount;

    public static RayTracer get()
    {
        return m_instance;
    }

    public void Awake()
    {
        if (m_instance != null) Destroy(this);
        m_instance = this;
        resolution = Mathf.Max(0.0f, resolution);
        Camera cam = Camera.main;
        m_rayCount.x = (int)(cam.pixelWidth * resolution);
        m_rayCount.y = (int)(cam.pixelHeight * resolution);
        m_rays = new Ray[m_rayCount.x, m_rayCount.y];

        for (int y = 0; y < m_rayCount.y; ++y)
        {
            for (int x = 0; x < m_rayCount.x; ++x)
            {
                m_rays[x, y] = new Ray(new Vector2(0, 0), new Vector2(0, 0), new Color(1, 1, 1));
            }
        }
        Debug.Log(m_rayCount);
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

        Camera cam = Camera.main;
        //amount of world units
        float ortho = cam.orthographicSize;
        Vector2 worldUnits = new Vector2( ortho * 2 * Screen.width / Screen.height, ortho * 2);


        int mod = m_rayCount.y / renderLines;

        for (int y = 0; y < m_rayCount.y; ++y)
        {
            //if ((y + m_renderLinesCycle) % mod != 0)
            //{
            //    continue;
            //}
            for (int x = 0; x < m_rayCount.x; ++x)
            {
                for (int i = 0; i < m_colliders.Count; ++i)
                {
                    Vector2 worldPos = cam.ScreenToWorldPoint(new Vector2(x/resolution, y/resolution));
                    if (m_colliders[i].pointOnSurface(worldPos))
                    {
                        m_colliders[i].setRay(m_rays[x, y], worldPos.x, worldPos.y);
                    }
                }
            }
        }

        for (int i = 0; i < m_rayCount.x * m_rayCount.y; ++i)
        {
            int x = i % m_rayCount.x;
            int y = i / m_rayCount.x;

            RayHit hit = collide(m_rays[x,y]);
            if (!hit.nullHit)
            {
                //if(m_rayHits == null) m_rayHits = new List<RayHit>();
                //m_rayHits.Add(hit);
            }
        }
        ++m_renderLinesCycle;
    }

    public void render()
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
        ray.reflect(hit);
        return hit;
    }
}
