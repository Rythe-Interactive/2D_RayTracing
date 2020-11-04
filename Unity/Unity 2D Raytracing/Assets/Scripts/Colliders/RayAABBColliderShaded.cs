using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RayAABBColliderShaded : RayCollider
{
    [SerializeField] Rect m_rect;
    [SerializeField] private List<RayHit> m_hits;
    [SerializeField] private Texture2D m_lightMapTexture;
    [SerializeField] private float m_lightMultiplier = 1.0f;
    [SerializeField] private bool m_isBackground = false;
    private Sprite m_sprite;
    private Material m_rayTracingOutlineMaterial;
    private float m_previousLightMultiplier;
    private Vector2 m_position;

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
        if(m_position != new Vector2(this.transform.position.x, this.transform.position.y))
        {
            m_position = this.transform.position;
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
        
        // Check if light is on the rect
        if (ray.position.x >= left && ray.position.x <= right && ray.position.y >= top && ray.position.y <= bottom)
        {
            Vector2 pixelOnBg = textureSpaceCoord(ray.position, m_sprite);
            hit = new RayHit(ray, ray.position, new Vector2Int((int)pixelOnBg.x, (int)pixelOnBg.y), ray.direction.normalized, this, m_sprite.texture.GetPixel((int)pixelOnBg.x, (int)pixelOnBg.y));
            hit.fromInsideShape = true;
            return true;
        }

        // Rect collision
        // 4 lines
        Vector2[] lineStart = new Vector2[2];
        Vector2[] lineEnd = new Vector2[2];
        Vector2 poi = new Vector2(0,0);
        Vector2 normal = new Vector2(0, 0);
        float toi = Mathf.Infinity;
        bool collision = false;
        // Determine the two lines that can be intersected for a certain ray
        if(ray.direction.x >= 0)
        {
            // Ray comes from left, going right
            if(ray.direction.y >= 0)
            {
                // Ray comes from left top
                lineStart[0] = leftTop;
                lineEnd[0] = rightTop;
                lineStart[1] = leftBottom;
                lineEnd[1] = leftTop;
            }
            else
            {
                // Ray comes from left bottom
                lineStart[0] = leftBottom;
                lineEnd[0] = leftTop;
                lineStart[1] = rightBottom;
                lineEnd[1] = leftBottom;
            }
        }
        else
        {
            // Ray comes from right, going left
            if (ray.direction.y >= 0)
            {
                // Ray comes from right top
                lineStart[0] = leftTop;
                lineEnd[0] = rightTop;
                lineStart[1] = rightTop;
                lineEnd[1] = rightBottom;
            }
            else
            {
                // Ray comes from right bottom
                lineStart[0] = rightTop;
                lineEnd[0] = rightBottom;
                lineStart[1] = rightBottom;
                lineEnd[1] = leftBottom;
            }
        }

        for(int i = 0; i < 2; ++i)
        {
            Vector2 startToStart = lineStart[i] - ray.position;
            Vector2 lineDir = lineEnd[i] - lineStart[i];
            float cross = FibonacciCross(ray.direction, lineDir);

            // calculate time of impact of ray onto line
            // Time of impact sould be 0-1
            float u = FibonacciCross(startToStart, ray.direction) / cross;
            // Time of impact on the ray
            // Because a ray is infinite it should be > 0
            float t = FibonacciCross(startToStart, lineDir) / cross;

            if(u > 0 && u < 1 && t > 0 && (!collision || t < toi))
            {
                // Collision
                collision = true;
                toi = t;
                poi = ray.position + ray.direction * t;
                normal.Set(-lineDir.y, lineDir.x);
            }
        }
        if (!collision)
        {
            hit = new RayHit(null);
            return false;
        }

        Vector2 pixel = textureSpaceCoord(poi, m_sprite);
        hit = new RayHit(ray, poi, new Vector2Int((int)pixel.x, (int)pixel.y), normal, this, m_sprite.texture.GetPixel((int)pixel.x, (int)pixel.y));
        if (m_isBackground) hit.fromInsideShape = true;
        return true;
    }

    public float FibonacciCross(Vector2 x, Vector2 y)
    {
        return (x.x * y.y) - (x.y * y.x);
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
            strength = ((hit.ray.intensity / distToRay) - 1) * Mathf.Abs(Vector2.Dot(hit.normal, hit.ray.direction)) * m_lightMultiplier;
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
        for (int i = 0; i < loops; ++i)
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
                float strengthForPixel = strength - ((pxlToPixel / m_sprite.pixelsPerUnit) * (1/m_lightMultiplier));
                if (strengthForPixel <= 0)
                {
                    // From here on out the pixels will not receive any light
                    // Therefore we can break form the loop
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
[CustomEditor(typeof(RayAABBColliderShaded))]
public class RayRectBackgroundColliderShadedEditor : Editor
{
    private RayAABBColliderShaded m_collider;

    public void OnSceneGUI()
    {
        m_collider = this.target as RayAABBColliderShaded;
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