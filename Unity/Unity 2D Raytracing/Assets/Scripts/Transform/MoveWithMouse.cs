using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MoveWithMouse : MonoBehaviour
{
    [SerializeField] public float radius;
    bool m_holding = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseOnThis() || m_holding)
        {
            if (Input.GetMouseButton(0))
            {
                m_holding = true;
            }
            else m_holding = false;
        }
        else m_holding = false;

        if (m_holding) moveToMouse();
    }

    bool mouseOnThis()
    {
        Vector2 mouse = mousePosition;
        if(Mathf.Abs((new Vector2(this.transform.position.x, this.transform.position.y) - mouse).magnitude) <= radius)
        {
            return true;
        }
        return false;
    }

    void moveToMouse()
    {
        Vector2 mouse = mousePosition;
        this.transform.position = new Vector3(mouse.x, mouse.y, this.transform.position.z);
    }

    Vector2 mousePosition
    {
        get
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}

#if (UNITY_EDITOR)
[CustomEditor(typeof(MoveWithMouse))]
public class MoveWithMouseEditor : Editor
{
    private MoveWithMouse m_collider;

    public void OnSceneGUI()
    {
        m_collider = this.target as MoveWithMouse;
        Handles.color = Color.red;
        Handles.DrawWireDisc(m_collider.transform.position, Vector3.forward, m_collider.radius);
    }
}
#endif