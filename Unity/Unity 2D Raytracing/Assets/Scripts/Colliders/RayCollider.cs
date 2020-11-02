using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventRayCollider : UnityEvent<RayCollider>
{

}

public abstract class RayCollider : MonoBehaviour
{
    [SerializeField] protected RayTracer m_tracer;
    [HideInInspector] [SerializeField] protected UnityEventRayCollider m_oncolliderChange;
    [HideInInspector] [SerializeField] protected bool m_changed = false;
    public void Start()
    {
        init();
        m_tracer.register(this);
    }
    protected abstract void init();
    public void Update()
    {
        m_changed = false;
        update();
        if(m_changed)
        {
            m_oncolliderChange?.Invoke(this);
        }
    }

    public void OnDestroy()
    {
        m_tracer.unRegister(this);
    }

    protected virtual void update() { }
    public abstract bool collide(Ray ray, out RayHit hit);
    public abstract void registerHit(RayHit hit);
    public abstract void clearHits();
    public abstract void applyHits();
    public void callBackOnChange(UnityAction<RayCollider> action)
    {
        if (m_oncolliderChange == null) m_oncolliderChange = new UnityEventRayCollider();
        m_oncolliderChange.AddListener(action);
    }
    public void removeCallBackOnChange(UnityAction<RayCollider> action)
    {
        m_oncolliderChange?.RemoveListener(action);
    }

    protected Vector2 textureSpaceCoord(Vector2 worldPos, Sprite sprite)
    {
        float ppu = sprite.pixelsPerUnit;

        Vector2 localPos = transform.InverseTransformPoint(worldPos) * ppu;

        Vector2 texSpacePivot = new Vector2(sprite.rect.x, sprite.rect.y) + sprite.pivot;
        Vector2 texSpaceCoord = texSpacePivot + localPos;

        return texSpaceCoord;
    }
}
