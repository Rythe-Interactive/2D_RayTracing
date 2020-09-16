using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RayCaster
{
    private static List<RayCollider> m_colliders;
    private static List<RayHit> m_rayHits;

    public static void register(RayCollider collider)
    {
        m_colliders.Add(collider);
    }

    public static void unRegister(RayCollider collider)
    {
        m_colliders.Remove(collider);
    }

    public static void render()
    {
        Camera cam = Camera.main;

    }
}
