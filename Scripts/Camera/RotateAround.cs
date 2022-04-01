using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [SerializeField] private Transform m_TransformToCircleAround;
    [SerializeField] private float m_RotationSpeed = 50f;
    [SerializeField] private float _axis;

    void Update()
    {
        

       // if (Input.GetKey(KeyCode.E))
       // {
       //     transform.RotateAround(m_TransformToCircleAround.position, -Vector3.up, 50 * Time.deltaTime);
       // }
       // if (Input.GetKey(KeyCode.Q))
       // {
       //     transform.RotateAround(m_TransformToCircleAround.position, Vector3.up, 50 * Time.deltaTime);
       // }
    }
}
