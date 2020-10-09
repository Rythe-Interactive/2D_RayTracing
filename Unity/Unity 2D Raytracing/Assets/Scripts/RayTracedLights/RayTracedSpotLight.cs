using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RayTracedSpotLight : RayTracedLight
{
    [SerializeField][Tooltip("In degrees")] float m_angle;
    private Quaternion m_rotation;

    protected override void init()
    {
        m_rotation = this.transform.rotation;
        for (int i = 0; i < m_rayCount; ++i)
        {
            float angle = m_angle / (float)m_rayCount * (float)i;
            Vector2 direction = this.transform.rotation * Quaternion.Euler(0, 0, (-m_angle / 2) + angle) * new Vector3(1, 0, 0);
            Ray ray = Ray.requestRay(this.transform.position, direction, this.gameObject.GetComponent<RayCollider>()); 
            m_rays.Add(ray);
            m_tracer.register(ray);
        }
    }

    protected override void update()
    {
        // If rayCount changed
        if (m_currentRayCount != m_rayCount)
        {
            m_hasChanged = true;
            if (m_useSpriteRendColor) m_color = m_spriteRend.color;
            m_position = this.transform.position;
            m_rotation = this.transform.rotation;
            for (int i = 0; i < Mathf.Max(m_currentRayCount, m_rayCount); ++i)
            {
                float angle = m_angle / (float)m_rayCount * (float)i;
                Vector2 direction = this.transform.rotation * Quaternion.Euler(0, 0, (-m_angle/2)+angle) * new Vector3(1, 0, 0);

                if (i >= m_currentRayCount)
                {
                    //Ray count has increased
                    Ray ray = Ray.requestRay(m_position, direction, this.GetComponent<RayCollider>());
                    m_tracer.register(ray);
                    m_rays.Add(ray);
                }
                else if (i >= m_rayCount)
                {
                    // RayCount has decreased
                    for (int r = m_currentRayCount - 1; r >= m_rayCount; --r) // Range that needs to be deleted
                    {
                        Ray ray = m_rays[r];
                        m_rays.RemoveAt(r);
                        m_tracer.unRegister(ray);
                        Ray.recycleRay(ray);
                    }
                    break;
                }
                else
                {
                    m_rays[i].reUse(m_position.x, m_position.y, direction.x, direction.y, this.gameObject.GetComponent<RayCollider>());
                }
            }

            m_currentRayCount = m_rayCount;
        }
        else // Certain things (like position) do not need changing if rays have been added
        {
            // If position changed
            Vector2 pos = this.transform.position;
            if (pos != m_position)
            {
                m_hasChanged = true;
                m_position = pos;
                for (int i = 0; i < m_rayCount; ++i)
                {
                    m_rays[i].setPosition(pos.x, pos.y);
                }
            }

            Quaternion rot = this.transform.rotation;
            if(m_rotation != rot)
            {
                m_hasChanged = true;
                m_rotation = rot;
                for(int i = 0; i < m_rayCount; ++i)
                {
                    float angle = m_angle / (float)m_rayCount * (float)i;

                    Vector2 direction = this.transform.rotation * Quaternion.Euler(0, 0, (-m_angle / 2) + angle) * new Vector3(1, 0, 0);
                    m_rays[i].setDirection(direction.x, direction.y);
                }
            }

            // If color changed
            bool colorChanged = false;
            if (!m_useSpriteRendColor && m_previousColor != m_color)
            {
                m_previousColor.r = m_color.r;
                m_previousColor.g = m_color.g;
                m_previousColor.b = m_color.b;
                m_previousColor.a = m_color.a;
                colorChanged = true;
            }
            if (m_useSpriteRendColor && m_spriteRend.color != m_color)
            {
                m_color.r = m_spriteRend.color.r;
                m_color.g = m_spriteRend.color.g;
                m_color.b = m_spriteRend.color.b;
                m_color.a = m_spriteRend.color.a;
                colorChanged = true;
            }
            if (colorChanged)
            {
                m_hasChanged = true;
                for (int i = 0; i < m_rayCount; ++i)
                {
                    m_rays[i].setColor(m_color.r, m_color.g, m_color.b, m_color.a);
                }
            }
        }
    }

    public Vector2 direction
    {
        get
        {
            Vector2 dir = this.transform.rotation * new Vector3(1, 0, 0);
            return dir;
        }
    }

    public Vector2 directionOfStart
    {
        get
        {
            Vector2 dir = this.transform.rotation * Quaternion.Euler(0, 0, -m_angle / 2) * new Vector3(1, 0, 0);
            return dir;
        }
    }

    public float angle
    {
        get
        {
            return m_angle;
        }
    }
}

[CustomEditor(typeof(RayTracedSpotLight))]
public class RayTracedSpotLightEditor : Editor
{
    private RayTracedSpotLight m_light;

    public void OnSceneGUI()
    {
        m_light = this.target as RayTracedSpotLight;
        Handles.color = Color.yellow;
        Handles.DrawSolidDisc(m_light.transform.position, new Vector3(0, 0, 1), 0.3f);
        Handles.color = Color.red;
        Handles.DrawSolidArc(m_light.transform.position, new Vector3(0, 0, 1), m_light.directionOfStart, m_light.angle, 0.3f);
    }
}