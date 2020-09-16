using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RayCollider : MonoBehaviour
{
    public abstract bool Collide(LightRay ray);
}
