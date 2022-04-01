using System.Collections;
using System.Collections.Generic;
using EVN.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
	[HideInEditorMode] public Entity m_EntityBelongingTo;
	[HideInEditorMode] public float m_MaxHealth;
	[HideInEditorMode] public float m_CurrentHealth;

	public void HealToFull()
	{
		m_CurrentHealth = m_MaxHealth;
	}

	public void TakeDamage(float damage)
	{
		m_CurrentHealth -= damage;
	}
}
