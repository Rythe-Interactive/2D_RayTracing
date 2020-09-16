using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTracedLight : MonoBehaviour
{
    [SerializeField][Range(4, 360)] private int m_rayCount = 10;

    private Ray[] m_rays;

    // Start is called before the first frame update
    void Start()
    {
        initRays();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        initRays();
    }

    private void initRays()
    {
        //m_rays = new Ray[m_rayCount];
        //for (int i = 0; i < m_rayCount; ++i)
        //{
        //    float angle = 360 / (float)m_rayCount * (float)i;
        //    Vector3 direction = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1)) * this.transform.rotation * new Vector3(1, 0, 0);
        //    Ray ray = new Ray(this.transform.position, direction);
        //    m_rays[i] = ray;
        //}
    }
}
