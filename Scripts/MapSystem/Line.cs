using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVN.MapSystem
{
    public class Line : MonoBehaviour
    {

        [SerializeField] private float m_Height;
        [ReadOnly] public RoomNode FromNode;
        [ReadOnly] public RoomNode ToNode;

        public void PositionLine()
        {
            // Position
            Vector3 center = Vector3.Lerp(FromNode.transform.position, ToNode.transform.position, 0.5f);
            transform.position = center;

            // Rotation
            Vector3 direction = FromNode.transform.position - ToNode.transform.position;
            transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);

            // Scale
            float distance = Vector2.Distance(FromNode.transform.position, ToNode.transform.position);
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(distance, m_Height);
        }
    }
}