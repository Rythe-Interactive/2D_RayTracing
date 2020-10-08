using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RayCollider : MonoBehaviour
{
    [SerializeField] protected RayTracer m_tracer;
    public void Start()
    {
        init();
        m_tracer.callBackOnLightAdd(onLightAdd);
    }
    protected abstract void init();
    public void Update()
    {
        update();
    }
    protected virtual void update() { }
    public abstract bool collide(Ray ray, out RayHit hit);
    public abstract void registerHit(RayHit hit);
    public abstract void clearHits();
    public abstract void applyHits();
    public virtual void onLightChange() { }
    public virtual void onLightAdd(RayTracedLight light)
    {
        light.callBackOnChange(onLightChange);
    }
    public virtual void onLightRemove(RayTracedLight light)
    {
        light.removeCallBackOnChange(onLightChange);
    }
}
