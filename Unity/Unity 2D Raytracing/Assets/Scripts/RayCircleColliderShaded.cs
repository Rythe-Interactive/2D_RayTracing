using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RayCircleColliderShaded : RayCollider
{
    [SerializeField] private float m_radius;
    [SerializeField] private readonly Vector2 m_offset;
    private Sprite m_sprite;
    private List<Ray> m_rays;
    [SerializeField] private List<RayHit> m_hits;
    [SerializeField] private Texture2D m_lightMapTexture;
    [SerializeField] private bool m_useExperimentalLightMapReset = false;
    private Material m_rayTracingOutlineMaterial;
    private Vector2 m_position;

    protected override void init()
    {
        m_tracer.register(this);
        SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
        m_sprite = renderer.sprite;
        m_rayTracingOutlineMaterial = renderer.material; // Create instance of material
        renderer.material = m_rayTracingOutlineMaterial;

        if(m_lightMapTexture == null) m_lightMapTexture = new Texture2D(m_sprite.texture.width, m_sprite.texture.height);
        m_lightMapTexture.minimumMipmapLevel = 2;
        m_lightMapTexture.requestedMipmapLevel = 3;
        for(int y = 0; y < m_lightMapTexture.height; ++y)
            for(int x = 0; x < m_lightMapTexture.width; ++x)
            {
                m_lightMapTexture.SetPixel(x, y, Color.clear);
            }
        m_position = this.transform.position;
        m_lightMapTexture.Apply();
        m_rayTracingOutlineMaterial.SetTexture("_MainTex", m_sprite.texture);
        m_rayTracingOutlineMaterial.SetTexture("_lightMapTexture", m_lightMapTexture);
        applyHits();
    }

    protected override void update()
    {
        if(m_useExperimentalLightMapReset && m_position != new Vector2(this.transform.position.x, this.transform.position.y))
        {
            m_position = this.transform.position;
            onLightChange();
        }
    }

    public void OnDestroy()
    {
        m_tracer.unRegister(this);
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

        float centerToRay = Vector2.Dot(dist, ray.normal.normalized);

        if (Mathf.Abs(centerToRay) > m_radius)
        {
            hit = new RayHit(ray);
            return false;
        }

        Vector2 closestRayPoint = ray.position + ray.direction * Vector2.Dot(dist, ray.direction.normalized);
        float closestRayPointToPoi = Mathf.Sqrt(Mathf.Pow(m_radius, 2) - Mathf.Pow(centerToRay, 2));

        Vector2 poi = closestRayPoint - (closestRayPointToPoi * ray.direction.normalized);
        Vector2 rayToPoi = poi - ray.position;
        if (rayToPoi.normalized != ray.direction)
        {
            hit = new RayHit(ray);
            return false;
        }
        Vector2 normal = (poi - center).normalized;
        Vector2 pixel = textureSpaceCoord(poi);
        hit = new RayHit(ray, poi, new Vector2Int((int)pixel.x, (int)pixel.y), normal, this, m_sprite.texture.GetPixel((int)pixel.x, (int)pixel.y));
        return true;
    }

    public override void registerHit(RayHit hit)
    {
        m_hits.Add(hit);
        m_lightMapTexture.SetPixel(hit.pixel.x, hit.pixel.y, hit.ray.color);
    }

    public override void applyHits()
    {
        m_lightMapTexture.Apply();
        if (!m_useExperimentalLightMapReset)
        {
            // Reset light map
            for (int y = 0; y < m_lightMapTexture.height; ++y)
                for (int x = 0; x < m_lightMapTexture.width; ++x)
                {
                    m_lightMapTexture.SetPixel(x, y, Color.clear);
                }
        }
    }

    public override void clearHits()
    {
        m_hits.Clear();
    }

    private Vector2 textureSpaceUV(Vector2 worldPos)
    {
        Texture2D tex = m_sprite.texture;
        Vector2 texSpaceCoord = textureSpaceCoord(worldPos);

        Vector2 uvs = texSpaceCoord;
        uvs.x /= tex.width;
        uvs.y /= tex.height;

        return uvs;
    }

    private Vector2 textureSpaceCoord(Vector2 worldPos)
    {
        float ppu = m_sprite.pixelsPerUnit;

        Vector2 localPos = transform.InverseTransformPoint(worldPos) * ppu;

        Vector2 texSpacePivot = new Vector2(m_sprite.rect.x, m_sprite.rect.y) + m_sprite.pivot;
        Vector2 texSpaceCoord = texSpacePivot + localPos;

        return texSpaceCoord;
    }

    public override void onLightChange()
    {
        Debug.Log("light changed");
        // Reset light map
        for (int y = 0; y < m_lightMapTexture.height; ++y)
            for (int x = 0; x < m_lightMapTexture.width; ++x)
            {
                m_lightMapTexture.SetPixel(x, y, Color.clear);
            }
        m_lightMapTexture.Apply();
    }

    public override void onLightAdd(RayTracedLight light)
    {
        if(m_rayTracingOutlineMaterial) light.callBackOnChange(onLightChange);
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