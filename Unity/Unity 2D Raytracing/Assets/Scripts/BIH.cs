using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class BIH
{
    List<RayCollider> m_colliders;

    public void addCollider(RayCollider collider)
    {
        if (m_colliders == null) m_colliders = new List<RayCollider>();
        m_colliders.Add(collider);
    }

    public void removeCollider(RayCollider collider)
    {
        m_colliders.Remove(collider);
    }

    public void build()
    {

    }
}
