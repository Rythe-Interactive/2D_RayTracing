using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventRayTracerLight : UnityEvent<RayTracedLight>
{

}


public abstract class RayTracedLight : MonoBehaviour
{
    [SerializeField] protected int m_rayCount;
    [SerializeField] protected RayTracer m_tracer;
    [SerializeField] protected Color m_color;
    [SerializeField] protected bool m_useSpriteRendColor;
    [SerializeField] protected float m_intensity = 1.0f;

    protected List<Ray> m_rays;
    protected Vector2 m_position;
    protected int m_currentRayCount;
    protected SpriteRenderer m_spriteRend;
    protected Color m_previousColor;
    protected bool m_hasChanged = false;
    protected bool m_startingUp = true;

    protected UnityEventRayTracerLight m_onLightChange;

    // Start is called before the first frame update
    void Start()
    {
        if(m_onLightChange == null) m_onLightChange = new UnityEventRayTracerLight();
        m_currentRayCount = m_rayCount;
        m_spriteRend = this.gameObject.GetComponent<SpriteRenderer>();
        if (m_useSpriteRendColor)
        {
            m_color = m_spriteRend.color;
        }
        m_rays = new List<Ray>();
        m_previousColor = new Color(m_color.r, m_color.g, m_color.b, m_color.a);
        init();
        m_tracer.register(this);
        m_startingUp = false;
    }

    // Update is called once per frame
    void Update()
    {
        m_hasChanged = false;
        update();
        if (m_hasChanged) m_onLightChange.Invoke(this);
    }

    public void OnDestroy()
    {
        OnDisable();
    }

    public void OnDisable()
    {
        if (m_rays != null)
        {
            for (int i = 0; i < m_rays.Count; ++i)
            {
                m_tracer.unRegister(m_rays[i]);
                RayVisualizer.instance.unRegister(m_rays[i]);
            }
        }
        m_tracer.unRegister(this);
    }

    public void OnEnable()
    {
        if (m_startingUp) return;
        if (m_rays != null)
        {
            for (int i = 0; i < m_rays.Count; ++i)
            {
                m_tracer.register(m_rays[i]);
                RayVisualizer.instance.register(m_rays[i]);
            }
        }
        m_tracer.register(this);
    }

    public bool hasChanged
    {
        get
        {
            return m_hasChanged;
        }
    }

    public float intensity
    {
        get
        {
            return m_intensity;
        }
    }

    public Color color
    {
        get
        {
            return m_color;
        }
    }

    public void callBackOnChange(UnityAction<RayTracedLight> action)
    {
        if (m_onLightChange == null) m_onLightChange = new UnityEventRayTracerLight();
        m_onLightChange.AddListener(action);
    }

    public void removeCallBackOnChange(UnityAction<RayTracedLight> action)
    {
        m_onLightChange?.RemoveListener(action);
    }

    abstract protected void init();
    abstract protected void update();
}
