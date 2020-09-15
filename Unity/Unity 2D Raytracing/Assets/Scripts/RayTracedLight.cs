using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTracedLight : MonoBehaviour
{
    [SerializeField][Range(4, 360)] private int m_rayCount = 10;

    private LightRay[] m_rays;

    // Start is called before the first frame update
    void Start()
    {
        initRays();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        initRays();
        for(int i = 0; i < m_rayCount; ++i)
        {
            m_rays[i].cast(true);
        }
    }

    private void initRays()
    {
        m_rays = new LightRay[m_rayCount];
        for (int i = 0; i < m_rayCount; ++i)
        {
            float angle = 360 / (float)m_rayCount * (float)i;
            Vector3 direction = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1)) * this.transform.rotation * new Vector3(1, 0, 0);
            LightRay ray = new LightRay(this.transform.position, direction);
            m_rays[i] = ray;
        }
    }
}
