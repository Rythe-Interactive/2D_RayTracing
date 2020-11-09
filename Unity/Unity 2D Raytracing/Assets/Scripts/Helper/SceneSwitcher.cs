using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] KeyCode m_key;
    [SerializeField] int m_sceneId = -1;
    [SerializeField] string m_scene;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(m_key))
        {
            if (m_sceneId != -1) SceneManager.LoadScene(m_sceneId);
            else SceneManager.LoadScene(m_scene);
        }
    }
}
