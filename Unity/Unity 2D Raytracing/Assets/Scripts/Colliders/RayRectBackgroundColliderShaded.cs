using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RayRectBackgroundColliderShaded : RayCollider
{
    [SerializeField] Rect m_rect;
    [SerializeField] private List<RayHit> m_hits;
    [SerializeField] private Texture2D m_lightMapTexture;
    [SerializeField] private float m_lightMultiplier = 1.0f;
    private Sprite m_sprite;
    private Material m_rayTracingOutlineMaterial;
    private float m_previousLightMultiplier;

    protected override void init()
    {
        SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
        m_sprite = renderer.sprite;
        m_rayTracingOutlineMaterial = renderer.material; // Create instance of material
        renderer.material = m_rayTracingOutlineMaterial;
        m_previousLightMultiplier = m_lightMultiplier;

        if (m_lightMapTexture == null) m_lightMapTexture = new Texture2D(m_sprite.texture.width, m_sprite.texture.height);
        m_lightMapTexture.minimumMipmapLevel = 2;
        m_lightMapTexture.requestedMipmapLevel = 3;
        for (int y = 0; y < m_lightMapTexture.height; ++y)
            for (int x = 0; x < m_lightMapTexture.width; ++x)
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
        if(m_lightMultiplier != m_previousLightMultiplier)
        {
            m_previousLightMultiplier = m_lightMultiplier;
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
        // Backgrounds only receive light when the light is directly on them
        if (ray.position.x >= left && ray.position.x <= right && ray.position.y >= top && ray.position.y <= bottom)
        {
            Vector2 pixelOnBg = textureSpaceCoord(ray.position, m_sprite);
            hit = new RayHit(ray, ray.position, new Vector2Int((int)pixelOnBg.x, (int)pixelOnBg.y), ray.direction.normalized, this, m_sprite.texture.GetPixel((int)pixelOnBg.x, (int)pixelOnBg.y));
            hit.fromInsideShape = true;
            return true;
        }
        //Debug.Log(ray.position);
        //Debug.Break();
        hit = new RayHit(null);
        return false;
    }

    public override void registerHit(RayHit hit)
    {
        m_hits.Add(hit);

        float distToRay = (hit.point - hit.ray.position).magnitude;
        float strength = 0;
        if (distToRay == 0)
        {
            strength = hit.ray.intensity * m_lightMultiplier;
        }
        else
        {
            strength = ((hit.ray.intensity / distToRay) - 1) * Mathf.Abs(Vector2.Dot(hit.normal, hit.ray.direction)) *m_lightMultiplier;
            if (strength <= 0)
            {
                return;
            }
        }
        //float maxDist = Mathf.Sqrt(Mathf.Pow(m_lightMapTexture.width, 2) + Mathf.Pow(m_lightMapTexture.height, 2));

        float maxDistWithLight = strength * m_sprite.pixelsPerUnit;
        int loops = Mathf.FloorToInt(maxDistWithLight) + 1;
        for (int i = 0; i < loops; ++i)
        {
            Vector2 pxl = hit.pixel + hit.ray.direction.normalized * i;
            if (pxl.x >= 0 && pxl.x < m_lightMapTexture.width &&
                pxl.y >= 0 && pxl.y < m_lightMapTexture.height)
            {
                float strengthForPixel = strength - (((hit.pixel - pxl).magnitude / m_sprite.pixelsPerUnit) * (1/m_lightMultiplier));
                if (strengthForPixel <= 0)
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

        // Reset light map
        for (int y = 0; y < m_lightMapTexture.height; ++y)
            for (int x = 0; x < m_lightMapTexture.width; ++x)
            {
                m_lightMapTexture.SetPixel(x, y, Color.clear);
            }
    }

    public override void clearHits()
    {
        m_hits.Clear();
    }

    public Rect rect
    {
        get
        {
            return m_rect;
        }
    }

    public Vector2 position
    {
        get
        {
            return this.transform.position;
        }
    }

    #region positions
    public Vector2 leftTop
    {
        get
        {
            return position - new Vector2(rect.width / 2, rect.height / 2);
        }
    }
    public Vector2 leftBottom
    {
        get
        {
            return position + new Vector2(-rect.width / 2, rect.height / 2);
        }
    }
    public Vector2 rightTop
    {
        get
        {
            return position + new Vector2(rect.width / 2, -rect.height / 2);
        }
    }
    public Vector2 rightBottom
    {
        get
        {
            return position + new Vector2(rect.width / 2, rect.height / 2);
        }
    }

    public float left
    {
        get
        {
            return position.x - rect.width / 2;
        }
    }

    public float right
    {
        get
        {
            return position.x + rect.width / 2;
        }
    }
    public float top
    {
        get
        {
            return position.y - rect.height / 2;
        }
    }

    public float bottom
    {
        get
        {
            return position.y + rect.height / 2;
        }
    }
    #endregion
}

#if (UNITY_EDITOR)
[CustomEditor(typeof(RayRectBackgroundColliderShaded))]
public class RayRectBackgroundColliderShadedEditor : Editor
{
    private RayRectBackgroundColliderShaded m_collider;

    public void OnSceneGUI()
    {
        m_collider = this.target as RayRectBackgroundColliderShaded;
        Handles.color = Color.red;
        Vector2 lt = m_collider.leftTop;
        Vector2 lb = m_collider.leftBottom;
        Vector2 rt = m_collider.rightTop;
        Vector2 rb = m_collider.rightBottom;
        Handles.DrawLine(lt, lb);
        Handles.DrawLine(lb, rb);
        Handles.DrawLine(rt, rt);
        Handles.DrawLine(rt, lt);

    }
}
#endif