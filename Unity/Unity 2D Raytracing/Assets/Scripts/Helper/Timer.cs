using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    float m_time = 0;
    [SerializeField] float m_pauseAtTime;
    [SerializeField] float m_stopAtTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
#if (UNITY_EDITOR)
        m_time += Time.deltaTime;
        if (m_time >= m_pauseAtTime && m_pauseAtTime > 0) Debug.Break();
        if (m_time >= m_stopAtTime && m_stopAtTime > 0) UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
