using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTowards : MonoBehaviour
{
    [SerializeField] GameObject m_faceTowards;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = m_faceTowards.transform.position - this.transform.position;
        this.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * 180/Mathf.PI);
    }
}
