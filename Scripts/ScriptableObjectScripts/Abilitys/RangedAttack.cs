using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = " RangedAttackBase", menuName = "StackedBeans/Abilities/Ranged/Base", order = -98)]
public class RangedAttack : Ability
{
	[HideInEditorMode]
	public Transform m_OriginTransform;

	[HideInEditorMode]
	public Transform m_TargetTransform;

	[HideInEditorMode] public LineRenderer m_LineRenderer;
	public void ManualSetValuesOnStart(Transform originTransform, LineRenderer lineRenderer)
	{
		base.ManualSetValuesOnStart();
		if (originTransform != null)
		{
			m_OriginTransform = originTransform;
		}

		if(lineRenderer != null)
		{
			m_LineRenderer = lineRenderer;
			m_LineRenderer.gameObject.SetActive(true);
		}

		if (m_LineRenderer == null)
		{
			m_LineRenderer = m_OriginTransform.gameObject.AddComponent<LineRenderer>();
		}
	}

	public void ManualSetValuesOnEnd()
	{
		base.ManualSetValuesOnEnd();
		if (m_LineRenderer != null)
		{
			m_LineRenderer.gameObject.SetActive(false);
		}
	}


	public void ManualSetTarget(Transform targetTransform)
	{
		base.ManualSetTarget();
		m_TargetTransform = targetTransform;
	}

	public override void ShowAbilityVisual()
	{
		base.ShowAbilityVisual();
		m_LineRenderer.SetPosition(0, m_OriginTransform.position);

		Vector3 worldPosition = Vector3.zero;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitData;

		if (Physics.Raycast(ray, out hitData, 1000))
		{
			worldPosition = hitData.point;
		}

		Vector3 targetPosition = worldPosition;

		LayerMask layerMask = ~0;

		if (Physics.Raycast(m_OriginTransform.position, (targetPosition -m_OriginTransform.position  ).normalized, out hitData, Mathf.Infinity, layerMask))
		{
			targetPosition = hitData.point;
			Debug.DrawRay(m_OriginTransform.position, (targetPosition - m_OriginTransform.position ).normalized * hitData.distance, Color.red);
		}

		if (targetPosition != Vector3.zero)
			m_LineRenderer.SetPosition(1, targetPosition);
		//m_LineRenderer.SetPosition(1, m_TargetTransform.position);
	}

}
