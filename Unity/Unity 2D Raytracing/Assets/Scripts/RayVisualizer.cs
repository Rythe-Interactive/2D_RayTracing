﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayVisualizer : MonoBehaviour
{
    private List<Ray> m_rays;
    public static RayVisualizer instance;
    [SerializeField] int m_rayCount;

    // Start is called before the first frame update
    void Start()
    {
        m_rays = new List<Ray>();
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    // Update is called once per frame
    public void Update()
    {
        for(int i = 0; i < m_rays.Count; ++i)
        {
            Debug.DrawRay(m_rays[i].position, m_rays[i].direction);
        }
        m_rayCount = m_rays.Count;
    }

    public void register(Ray ray)
    {
        m_rays.Add(ray);
    }
}