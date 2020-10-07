using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RayTracedLight : MonoBehaviour
{
    [SerializeField] protected int m_rayCount;
    [SerializeField] protected RayTracer m_tracer;
    [SerializeField] protected Color m_color;
    [SerializeField] protected bool m_useSpriteRendColor;

    protected List<Ray> m_rays;
    protected Vector2 m_position;
    protected int m_currentRayCount;
    protected SpriteRenderer m_spriteRend;
    protected Color m_previousColor;

    // Start is called before the first frame update
    void Start()
    {
        m_currentRayCount = m_rayCount;
        if (m_useSpriteRendColor)
        {
            m_spriteRend = this.gameObject.GetComponent<SpriteRenderer>();
            m_color = m_spriteRend.color;
        }
        m_rays = new List<Ray>();
        m_previousColor = new Color(m_color.r, m_color.g, m_color.b, m_color.a);
        init();
    }

    // Update is called once per frame
    void Update()
    {
        update();
    }

    abstract protected void init();
    abstract protected void update();
}
