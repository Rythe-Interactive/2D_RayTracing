using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTracer : MonoBehaviour
{
    private List<RayCollider> m_colliders;
    private List<RayHit> m_rayHits;
    private Ray[,] m_rays;
    private static RayTracer m_instance;
    public float resolution = 1.0f;
    private Vector2Int m_rayCount;
    [SerializeField] Camera m_mainCamera;

    public static RayTracer get()
    {
        return m_instance;
    }

    public void Awake()
    {
        if (m_instance != null) Destroy(this);
        m_instance = this;
        resolution = Mathf.Max(0.0f, resolution);
        m_rayCount.x = (int)(m_mainCamera.pixelWidth * resolution);
        m_rayCount.y = (int)(m_mainCamera.pixelHeight * resolution);
        m_rays = new Ray[m_rayCount.x, m_rayCount.y];

        for (int y = 0; y < m_rayCount.y; ++y)
        {
            for (int x = 0; x < m_rayCount.x; ++x)
            {
                m_rays[x, y] = new Ray(new Vector2(0, 0), new Vector2(0, 0), new Color(0, 0, 0, 0));
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

        //amount of world units
        float ortho = m_mainCamera.orthographicSize;
        Vector2 worldUnits = new Vector2( ortho * 2 * Screen.width / Screen.height, ortho * 2);

        // Doing a for loop like this over a double for loop is a little faster
        // FPS increase by 0.2
        for (int xy = 0; xy < m_rayCount.y*m_rayCount.x; ++xy)
        {
            int x = xy % m_rayCount.x;
            int y = xy / m_rayCount.x;
            //if ((y + m_renderLinesCycle) % mod != 0)
            //{
            //    continue;
            //}
            bool rayHit = false;
            for (int i = 0; i < m_colliders.Count; ++i)
            {
                Vector2 worldPos = m_mainCamera.ScreenToWorldPoint(new Vector2(x/resolution, y/resolution));
                if (m_colliders[i].pointOnSurface(worldPos))
                {
                    m_colliders[i].setRay(m_rays[x, y], worldPos.x, worldPos.y);
                    rayHit = true;
                }
            }
            if (!rayHit)
            {
                m_rays[x, y].reUse(0, 0, 0, 0, 0, 0, 0, 0, 0);
                m_rays[x, y].resetReflect();
                //Debug.Log("No hit! - " + x + ";" + y);
            }
        }

        //Debug.Log("--------------------------------------------------------");

        for (int i = 0; i < m_rayCount.x * m_rayCount.y; ++i)
        {
            int x = i % m_rayCount.x;
            int y = i / m_rayCount.x;

            RayHit hit = collide(m_rays[x,y]);
            if (!hit.nullHit)
            {
                m_rays[x, y].setColor(1, 0, 0);
                //if(m_rayHits == null) m_rayHits = new List<RayHit>();
                //m_rayHits.Add(hit);
            }
        }
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
