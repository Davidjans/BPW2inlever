using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace EVN
{
	public class MultipleTargetsCamera : MonoBehaviour
	{
		[SerializeField]
		private Vector3 m_OffsetVector;
		[SerializeField]
		private float m_MinimumDistance = 10, m_DistanceMultiplier = 2;
		[SerializeField]
		private float m_SmoothTime = 0.5f;

		private Vector3 m_Velocity;
		public List<Transform> m_Targets = new List<Transform>();

		public void SetTargets(List<Transform> targets)
		{
			m_Targets = targets.ToList();
		}

		private void LateUpdate()
		{
			if (m_Targets.Count == 0)
			{
				//Debug.LogWarning("MultipleTargetsCamera attached to " + name + " does not have any targets.");
				return;
			}

			m_Targets.RemoveAll(transform => !transform.gameObject.activeSelf); // Remove all transforms from the list that are not active.
			Bounds targetBounds = GetTargetBounds();
			Vector3 newPosition = (m_OffsetVector.normalized * Mathf.Max(targetBounds.extents.magnitude * m_DistanceMultiplier, m_MinimumDistance)) + targetBounds.center;
			transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref m_Velocity, m_SmoothTime);
		}

		private Bounds GetTargetBounds()
		{
			if (m_Targets.Count == 1)
			{
				return new Bounds(m_Targets[0].transform.position, Vector3.zero);
			}
			else
			{
				Bounds bounds = new Bounds(m_Targets[0].transform.position, Vector3.zero);
				for (int i = 1; i < m_Targets.Count; i++)
				{
					bounds.Encapsulate(m_Targets[i].transform.position);
				}
				return bounds;
			}
		}
		public void OnEnable()
		{
			EntityManager.Instance.AddMultiCamera(this);
		}

		public void OnDisable()
		{
			EntityManager.Instance.RemoveMultiCamera(this);
		}
	}
}
