using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceReTraceOnPress : MonoBehaviour
{
    [SerializeField] RayTracer m_tracer;
    [SerializeField] KeyCode m_key;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(m_key))
        {
            m_tracer.forceReTrace();
        }
    }
}
