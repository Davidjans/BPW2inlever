using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
public class LookAtPlayer : MonoBehaviour {

    [SerializeField] private Transform m_Camera;
    [SerializeField] private Vector3 RotationOffset;

    // Update is called once per frame

    private void Start()
	{
        if(m_Camera == null)
		{
            m_Camera = Camera.main.transform;
		}
	}

    void FixedUpdate () {
        Quaternion newRotation = Quaternion.LookRotation((m_Camera.position - transform.position).normalized);
        transform.rotation = newRotation;
        transform.eulerAngles += RotationOffset;
        
    }
}
