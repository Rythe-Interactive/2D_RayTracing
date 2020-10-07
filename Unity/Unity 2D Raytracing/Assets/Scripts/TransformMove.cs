using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class TransformMove : MonoBehaviour
{
    [SerializeField] private KeyCode m_moveLeft = KeyCode.A;
    [SerializeField] private KeyCode m_moveRight = KeyCode.D;
    [SerializeField] private KeyCode m_moveUp = KeyCode.W;
    [SerializeField] private KeyCode m_moveDown = KeyCode.S;
    [SerializeField] private float m_speed = 1.0f;

    private Transform m_transform;

    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(m_moveLeft)) m_transform.position += new Vector3(-m_speed*Time.deltaTime, 0, 0);
        if (Input.GetKey(m_moveRight)) m_transform.position += new Vector3(m_speed*Time.deltaTime, 0, 0);
        if (Input.GetKey(m_moveUp)) m_transform.position += new Vector3(0, m_speed*Time.deltaTime, 0);
        if (Input.GetKey(m_moveDown)) m_transform.position += new Vector3(0, -m_speed*Time.deltaTime, 0);
    }
}
