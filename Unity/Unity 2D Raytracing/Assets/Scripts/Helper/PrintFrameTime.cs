using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PrintFrameTime : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_fpsTextField;
    [SerializeField] TextMeshProUGUI m_frameTimeTextField;
    float m_timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_timer += Time.unscaledDeltaTime;
        if (m_timer >= 0.5f)
        {
            Color color = new Color(1, 1 * (0.01666f /Time.unscaledDeltaTime), 1 * (0.01666f / Time.unscaledDeltaTime), 1);
            m_fpsTextField.color = color;
            m_frameTimeTextField.color = color;
            m_fpsTextField.text = "FPS:           " + 1 / Time.unscaledDeltaTime;
            m_frameTimeTextField.text = "FrameTime " + Time.unscaledDeltaTime * 1000;
            m_timer = 0.0f;
        }
    }
}
