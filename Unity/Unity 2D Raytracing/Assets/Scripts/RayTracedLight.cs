using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTracedLight : MonoBehaviour
{
    [SerializeField] private int rayCount = 10;
    [SerializeField] private RayTracer m_tracer;
    [SerializeField] private Color m_color;
    [SerializeField] private bool m_useSpriteRendColor;

    private List<Ray> m_rays;
    private Vector2 m_position;
    private int m_rayCount;
    private SpriteRenderer m_spriteRend;
    private Color m_previousColor;

    // Start is called before the first frame update
    void Start()
    {
        m_rayCount = rayCount;
        m_rays = new List<Ray>();
        for (int i = 0; i < m_rayCount; ++i)
        {
            float angle = 360 / (float)m_rayCount * (float)i;
            Vector3 direction = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1)) * this.transform.rotation * new Vector3(1, 0, 0);
            Ray ray = new Ray(this.transform.position, direction, this.gameObject.GetComponent<RayCollider>());
            m_rays.Add(ray);
            m_tracer.register(ray);
        }

        if (m_useSpriteRendColor)
        {
            m_spriteRend = this.gameObject.GetComponent<SpriteRenderer>();
            m_color = m_spriteRend.color;
        }
        m_previousColor = new Color(m_color.r, m_color.g, m_color.b, m_color.a);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // If rayCount changed
        if (m_rayCount != rayCount)
        {
            Debug.Log("Raycount changed from: " + m_rayCount + " to: " + rayCount);
            if (m_useSpriteRendColor) m_color = m_spriteRend.color;
            m_position = this.transform.position;
            for (int i = 0; i < Mathf.Max(rayCount, m_rayCount); ++i)
            {
                Debug.Log("\tComputing index " + i);
                float angle = 360 / (float)rayCount * (float)i;
                Debug.Log("\tAngle: " + angle);
                Vector3 direction = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1)) * this.transform.rotation * new Vector3(1, 0, 0);
                Ray ray;

                if (i >= m_rayCount)
                {
                    Debug.Log("\tIndex is higher than previous count");
                    // RayCount has increased and the ray should be added to the list
                    if (i >= m_rays.Count)
                    {
                        Debug.Log("\tIndex out of bounds for list --> adding");
                        // The list is not yet big enough, thus increase the size
                        ray = new Ray(m_position, direction, this.gameObject.GetComponent<RayCollider>());
                        m_rays.Add(ray);
                    }
                    else
                    {
                        Debug.Log("\tIndex does still exist in list --> reusing ray");
                        // The list still has unused elements
                        m_rays[i].reUse(m_position.x, m_position.y, direction.x, direction.y, this.GetComponent<RayCollider>());
                        ray = m_rays[i];
                    }
                    m_tracer.register(ray);
                }
                else if (i >= rayCount)
                {
                    Debug.Log("\tIndex is higher than new ray count --> resetting ray");
                    // RayCount has decreased
                    // Rays will not be removed from the list, but will be reset completely
                    m_rays[i].reset();
                    m_tracer.unRegister(m_rays[i]);
                }
                else
                {
                    Debug.Log("\tIndex exists in both ray counts --> reusing ray");
                    m_rays[i].reUse(m_position.x, m_position.y, direction.x, direction.y, this.gameObject.GetComponent<RayCollider>());
                }
            }

            m_rayCount = rayCount;
        }
        else // Certain things (like position) do not need changing if rays have been added
        {
            // If position changed
            Vector2 pos = this.transform.position;
            if (pos != m_position)
            {
                m_position = pos;
                for (int i = 0; i < m_rayCount; ++i)
                {
                    m_rays[i].setPosition(pos.x, pos.y);
                }
            }

            // If color changed
            bool colorChanged = false;
            if(!m_useSpriteRendColor && m_previousColor != m_color)
            {
                m_previousColor.r = m_color.r;
                m_previousColor.g = m_color.g;
                m_previousColor.b = m_color.b;
                m_previousColor.a = m_color.a;
                colorChanged = true;
            }
            if(m_useSpriteRendColor && m_spriteRend.color != m_color)
            {
                m_color.r = m_spriteRend.color.r;
                m_color.g = m_spriteRend.color.g;
                m_color.b = m_spriteRend.color.b;
                m_color.a = m_spriteRend.color.a;
                colorChanged = true;
            }
            for(int i = 0; i < m_rayCount; ++i)
            {
                m_rays[i].setColor(m_color.r, m_color.g, m_color.b, m_color.a);
            }
        }


    }
}
