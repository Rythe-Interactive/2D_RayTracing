using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RayTracer
{
    private static List<RayCollider> m_colliders;
    private static List<RayHit> m_rayHits;
    private static Ray[,] m_rays;

    public static void Start()
    {
        Camera cam = Camera.main;
        m_rays = new Ray[cam.pixelWidth, cam.pixelHeight];

        for (int y = 0; y < cam.pixelHeight; ++y)
        {
            for (int x = 0; x < cam.pixelWidth; ++x)
            {
                m_rays[x, y] = new Ray(new Vector2(0, 0), new Vector2(0, 0), new Color(1, 1, 1));
            }
        }
    }

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

        Camera cam = Camera.main;
        //amount of world units
        Vector2 worldUnits = new Vector2(
            cam.orthographicSize*2*Screen.width/Screen.height,
            cam.orthographicSize * 2);

        for (int y = 0; y < cam.pixelHeight; ++y)
        {
            for(int x = 0; x < cam.pixelWidth; ++x)
            {
                for(int i = 0; i < m_colliders.Count; ++i)
                {
                    Vector2 worldPos = cam.ScreenToWorldPoint(new Vector2(x, y));
                    if (m_colliders[i].pointOnSurface(worldPos))
                    {
                        m_colliders[i].setRay(m_rays[x, y], worldPos.x, worldPos.y);
                    }
                }
            }
        }

        for (int i = 0; i < cam.pixelWidth*cam.pixelHeight; ++i)
        {
            int x = i % cam.pixelWidth;
            int y = i / cam.pixelWidth;

            RayHit hit = collide(m_rays[x,y]);
            if (!hit.nullHit)
            {
                //if(m_rayHits == null) m_rayHits = new List<RayHit>();
                //m_rayHits.Add(hit);
            }
        }
    }

    public static RayHit collide(Ray ray)
    {
        if (m_colliders == null) return new RayHit(ray);
        RayHit hit = new RayHit(ray);
        float dist = 0;
        for (int i = 0; i < m_colliders.Count; ++i)
        {
            if(m_colliders[i].collide(ray, out RayHit newHit))
            {
                //Debug.Log("HITS!");
                if(!hit.nullHit || (newHit.point-ray.position).magnitude < dist)
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
