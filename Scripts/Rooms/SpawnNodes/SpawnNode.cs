using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using StackedBeans.Utils;
using Unity.Mathematics;

public class SpawnNode : MonoBehaviour
{
	[SerializeField] protected bool m_OnStart = true;
	[SerializeField] protected GameObject m_NodeItem;
	[HideInEditorMode][SerializeField] protected GameObject m_SpawnedObject;
	[SerializeField] protected float m_DestroyAfterTime = 5;
	protected virtual void Start()
	{
		transform.DestroyVisualChildren();
		if (m_OnStart)
		{
			SpawnNodeItem();
		}
	}

	public virtual void SpawnNodeItem()
	{
		m_SpawnedObject = Instantiate(m_NodeItem, transform.position, transform.rotation);
		Destroy(this.gameObject,m_DestroyAfterTime);
	}
}
