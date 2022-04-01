using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class RadialLayoutGroup : LayoutGroup
{
    [FoldoutGroup("Scale")]
    [SerializeField]
    private Vector2 m_Size;


    [FoldoutGroup("Position")]
    [SerializeField]
    private float m_DistanceFromPoint;

    [FoldoutGroup("Position")]
    [LabelText("Offset °")]
    [Range(0, 360)]
    [SerializeField]
    private float m_Offset;

    [FoldoutGroup("Position")]
    [LabelText("m_Space °")]
    [Range(0, 360)]
    [SerializeField]
    private float m_Space;

    [FoldoutGroup("Position")]
    [SerializeField]
    private bool m_FromMiddle;


    [FoldoutGroup("Rotation")]
    [SerializeField]
    private bool m_RotateToCenter;

    [LabelText("Rotation Offset °")]
    [FoldoutGroup("Rotation")]
    [Range(0, 360)]
    [SerializeField]
    private float m_RotationOffset;



    public override void SetLayoutHorizontal() { }

    public override void SetLayoutVertical() { }

    protected override void OnEnable()
    {
        base.OnEnable();
        CalculateRadialLayout();
    }

    public override void CalculateLayoutInputVertical()
    {
        CalculateRadialLayout();
    }

    public override void CalculateLayoutInputHorizontal()
    {
        CalculateRadialLayout();
    }


    /// <summary>
    /// Calculates and positions all the child rect transforms
    /// </summary>
    public void CalculateRadialLayout()
    {

        if (transform.childCount == 0)
            return;

        m_Tracker.Clear();

        // Get the all the children
        List<RectTransform> childs = new List<RectTransform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {

                childs.Add((RectTransform)transform.GetChild(i).transform);
            }
        }


        // Position all the children
        for (int i = 0; i < childs.Count; i++)
        {
            // Get the child and check if its not null
            RectTransform child = childs[i];

            if (child == null)
                continue;


            float currentChild = i - (m_FromMiddle ? ((childs.Count - 1) / 2f) : 0);
            float radians = (2 * Mathf.PI / childs.Count * currentChild * (m_Space / 360f)) + (m_Offset / 360f) * (Mathf.PI * 2);

            // Get the spawn direction
            Vector2 spawnDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

            // Get the rotation angle
            float angle = m_RotationOffset;

            if (m_RotateToCenter)
            {
                angle += Mathf.Atan2(spawnDirection.y, spawnDirection.x) * Mathf.Rad2Deg;
            }


            // Apply all the positions and rotations etc
            child.localPosition = spawnDirection * m_DistanceFromPoint;
            child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);
            child.sizeDelta = m_Size;
            child.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            m_Tracker.Add(this, child, DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.Pivot);
        }
    }
}
