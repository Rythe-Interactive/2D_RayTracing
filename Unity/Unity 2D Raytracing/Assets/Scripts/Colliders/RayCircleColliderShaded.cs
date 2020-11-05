using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RayCircleColliderShaded : RayCollider
{
    [SerializeField] private float m_radius;
    [SerializeField] private readonly Vector2 m_offset;
    [SerializeField] private List<RayHit> m_hits;
    [SerializeField] private Texture2D m_lightMapTexture;
    private Sprite m_sprite;
    private Material m_rayTracingOutlineMaterial;
    private Vector2 m_position;
    private Quaternion m_rotation;

    protected override void init()
    {
        SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
        m_sprite = renderer.sprite;
        m_rayTracingOutlineMaterial = renderer.material; // Create instance of material
        renderer.material = m_rayTracingOutlineMaterial;

        m_position = this.transform.position;

        if(m_lightMapTexture == null) m_lightMapTexture = new Texture2D(m_sprite.texture.width, m_sprite.texture.height);
        m_lightMapTexture.minimumMipmapLevel = 2;
        m_lightMapTexture.requestedMipmapLevel = 3;
        for(int y = 0; y < m_lightMapTexture.height; ++y)
            for(int x = 0; x < m_lightMapTexture.width; ++x)
            {
                m_lightMapTexture.SetPixel(x, y, Color.clear);
            }

        m_lightMapTexture.filterMode = FilterMode.Trilinear;
        m_lightMapTexture.anisoLevel = 4;
        m_lightMapTexture.Apply(true);
        m_rayTracingOutlineMaterial.SetTexture("_MainTex", m_sprite.texture);
        m_rayTracingOutlineMaterial.SetTexture("_lightMapTexture", m_lightMapTexture);
        m_rayTracingOutlineMaterial.SetFloat("_MipLevel", 5);
        applyHits();
        m_hits = new List<RayHit>();
    }

    protected override void update()
    {
        if(m_position != new Vector2(this.transform.position.x, this.transform.position.y))
        {
            m_position = this.transform.position;
            m_changed = true;
        }
        
        if (m_rotation != this.transform.rotation)
        {
            m_rotation = this.transform.rotation;
            m_changed = true;
        }
    }

    public override bool collide(Ray ray, out RayHit hit)
    {
        if (ray == null || ray.origin == this)
        {
            hit = new RayHit(null);
            return false;
        }
        Vector2 pos = center;
        Vector2 dist = pos - ray.position; // Distance between ray start and circle center

        // Test if ray starts in circle
        if (Mathf.Abs(dist.magnitude) < m_radius)
        {
            Vector2 pixelOnCircle = textureSpaceCoord(ray.position, m_sprite);
            hit = new RayHit(ray, ray.position, new Vector2Int((int)pixelOnCircle.x, (int)pixelOnCircle.y), ray.direction.normalized, this, m_sprite.texture.GetPixel((int)pixelOnCircle.x, (int)pixelOnCircle.y));
            hit.fromInsideShape = true;
            return true;
        }

        // Projection of ray onto the distance between ray start and circle center
        // Gives the distance between the center of the circle and the closest point on the ray
        float centerToRay = Vector2.Dot(dist, ray.normal.normalized);

        if (Mathf.Abs(centerToRay) > m_radius)
        {
            hit = new RayHit(ray);
            return false;
        }

        Vector2 closestRayPoint = ray.position + ray.direction * Vector2.Dot(dist, ray.direction.normalized);
        float closestRayPointToPoi = Mathf.Sqrt(Mathf.Pow(m_radius, 2) - Mathf.Pow(centerToRay, 2));

        Vector2 poi = closestRayPoint - (closestRayPointToPoi * ray.direction.normalized);
        if ((poi - ray.position).magnitude > ray.intensity)
        {
            hit = new RayHit(null);
            return false;
        }
        Vector2 rayToPoi = poi - ray.position;
        if (rayToPoi.normalized != ray.direction)
        {
            hit = new RayHit(ray);
            return false;
        }
        Vector2 normal = (poi - center).normalized;
        Vector2 pixel = textureSpaceCoord(poi, m_sprite);
        hit = new RayHit(ray, poi, new Vector2Int((int)pixel.x, (int)pixel.y), normal, this, m_sprite.texture.GetPixel((int)pixel.x, (int)pixel.y));
        return true;
    }

    public override void registerHit(RayHit hit)
    {
        m_hits.Add(hit);

        float distToRay = (hit.point - hit.ray.position).magnitude;
        float strength = 0;
        if (distToRay == 0)
        {
            strength = hit.ray.intensity;
        }
        else
        {
            strength = ((hit.ray.intensity / distToRay) - 1) * Mathf.Abs(Vector2.Dot(hit.normal, hit.ray.direction));
            if (strength <= 0)
            {
                return;
            }
        }

        float distToEnd = 0;
        if (hit.ray.hasBounce() && hit.ray.getBounce().origin != this)
        {
            Vector2 end = hit.ray.getBounce().position;
            distToEnd = Mathf.Abs((end - hit.ray.position).magnitude) * m_sprite.pixelsPerUnit;
        }

        float maxDistWithLight = strength * m_sprite.pixelsPerUnit;
        int loops = Mathf.FloorToInt(maxDistWithLight) + 1;
        for(int i = 0; i < loops; ++i)
        {
            Vector2 pxl = hit.pixel + hit.ray.direction.normalized * i;
            if (pxl.x >= 0 && pxl.x < m_lightMapTexture.width &&
                pxl.y >= 0 && pxl.y < m_lightMapTexture.height)
            {
                float pxlToPixel = (hit.pixel - pxl).magnitude;
                if (distToEnd != 0 && Mathf.Abs(pxlToPixel) >= distToEnd)
                {
                    break;
                }
                float strengthForPixel = strength - pxlToPixel / m_sprite.pixelsPerUnit;
                if(strengthForPixel <= 0)
                {
                    // From here on out the pixels will not receive any light
                    break; 
                }
                m_lightMapTexture.SetPixel((int)pxl.x, (int)pxl.y, m_lightMapTexture.GetPixel((int)pxl.x, (int)pxl.y) + hit.ray.color * strengthForPixel);
            }
        }
    }

    public override void applyHits()
    {
        m_lightMapTexture.filterMode = FilterMode.Trilinear;
        m_lightMapTexture.anisoLevel = 4;
        m_lightMapTexture.Apply(true);
    }

    public override void clearHits()
    {
        m_hits.Clear();

        // Reset light map
        for (int y = 0; y < m_lightMapTexture.height; ++y)
            for (int x = 0; x < m_lightMapTexture.width; ++x)
            {
                m_lightMapTexture.SetPixel(x, y, Color.clear);
            }
    }

    public Vector2 center
    {
        get
        {
            return new Vector2(this.transform.position.x, this.transform.position.y) + m_offset;
        }
    }

    public float radius
    {
        get
        {
            return m_radius;
        }
    }
}

#if (UNITY_EDITOR)
[CustomEditor(typeof(RayCircleColliderShaded))]
public class RayCircleColliderShadedEditor : Editor
{
    private RayCircleColliderShaded m_collider;

    public void OnSceneGUI()
    {
        m_collider = this.target as RayCircleColliderShaded;
        Handles.color = Color.red;
        Handles.DrawWireDisc(m_collider.center, Vector3.forward, m_collider.radius);
    }
}
#endif