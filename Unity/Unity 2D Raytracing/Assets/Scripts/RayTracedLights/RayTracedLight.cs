using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    protected bool m_hasChanged = false;

    protected UnityEvent m_onLightChange;

    // Start is called before the first frame update
    void Start()
    {
        m_onLightChange = new UnityEvent();
        m_tracer.register(this);
        m_currentRayCount = m_rayCount;
        m_spriteRend = this.gameObject.GetComponent<SpriteRenderer>();
        if (m_useSpriteRendColor)
        {
            m_color = m_spriteRend.color;
        }
        m_rays = new List<Ray>();
        m_previousColor = new Color(m_color.r, m_color.g, m_color.b, m_color.a);
        init();
    }

    // Update is called once per frame
    void Update()
    {
        m_hasChanged = false;
        update();
        if (m_hasChanged) m_onLightChange.Invoke();
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
            }
        }
        m_tracer.unRegister(this);
    }

    public void OnEnable()
    {
        if (m_rays != null)
        {
            for (int i = 0; i < m_rays.Count; ++i)
            {
                m_tracer.register(m_rays[i]);
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

    public void callBackOnChange(UnityAction action)
    {
        m_onLightChange.AddListener(action);
    }

    public void removeCallBackOnChange(UnityAction action)
    {
        m_onLightChange.RemoveListener(action);
    }

    abstract protected void init();
    abstract protected void update();
}
