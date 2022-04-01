using System;
using System.Collections;
using System.Collections.Generic;
using EVN.Entities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;



[CreateAssetMenu(fileName = "Base", menuName = "StackedBeans/AI/Offense/Base", order = -99)]
public class AIOffense : ScriptableObject
{
	[HideInEditorMode] public Entity m_BelongTo;

	[HideInEditorMode] public List<Entity> m_PotentionalTargets = new List<Entity>();

	public EnemyTargetting m_TargettingMethod = EnemyTargetting.Closest;

	public void GetPotentionalTargets()
	{
		m_PotentionalTargets = new List<Entity>();
		foreach (var item in CombatManager.Instance.m_EntitiesInCombat)
		{
			if(item.m_EntityFaction != m_BelongTo.m_EntityFaction)
				m_PotentionalTargets.Add(item);
		}	
	}

	public Entity SelectTarget()
	{
		GetPotentionalTargets();

		Entity currentTarget = null;

		switch (m_TargettingMethod)
		{
			case EnemyTargetting.LowestHPPercentage:
				currentTarget = LowestHPTargetSelection();
				break;
			case EnemyTargetting.LowestHpStatic:
				currentTarget = LowestHPPercentageTargetSelection();
				break;
			case EnemyTargetting.Farthest:
				currentTarget = FurthestTargetSelection();
				break;
			case EnemyTargetting.Closest:
				currentTarget = ClosestTargetSelection();
				break;
			case EnemyTargetting.CompleteRandom:
				break;
			case EnemyTargetting.LowestMagicArmor:
				break;
			case EnemyTargetting.LowestPhysicalArmor:
				break;
			case EnemyTargetting.LowestMovementSpeed:
				break;
			case EnemyTargetting.HighestMovementSpeed:
				break;
			case EnemyTargetting.PrimaryRangedAttacker:
				break;
			case EnemyTargetting.PrimarymelleeAttacker:
				break;
			default:
				break;
		}
		UIManager.Instance.m_TargetInfo.SetTargets(m_BelongTo,currentTarget);
		return currentTarget;
	}

	#region SlectionMethods

	public Entity LowestHPTargetSelection()
	{
		float currenthp = 999999999999999;

		Entity CurrentTarget = null;

		foreach (var item in m_PotentionalTargets)
		{
			float currentHealth = item.m_AttributeDictionary["Health"].m_CurrentValue;
			if(currentHealth < currenthp)
			{
				currenthp = currentHealth;
				CurrentTarget = item;
			}
		}

		return CurrentTarget;
	}

	public Entity LowestHPPercentageTargetSelection()
	{
		float currenthp = 999999999999999;

		Entity CurrentTarget = null;

		foreach (var item in m_PotentionalTargets)
		{
			if (!item.m_AttributeDictionary.ContainsKey("Health"))
				continue;
			float currentPercentage = item.m_AttributeDictionary["Health"].m_CurrentValue /
			                          item.m_AttributeDictionary["Health"].m_CurrentMaxValue;
			if ( currentPercentage < currenthp)
			{
				currenthp = currentPercentage;
				CurrentTarget = item;
			}
		}

		return CurrentTarget;
	}

	public Entity FurthestTargetSelection()
	{
		float currentDistance = 0;

		Entity CurrentTarget = null;

		float dist = 0;
		foreach (var item in m_PotentionalTargets)
		{
			dist = Vector3.Distance(m_BelongTo.transform.position, item.transform.position);
			if ( dist > currentDistance)
			{
				currentDistance = dist;
				CurrentTarget = item;
			}
		}

		return CurrentTarget;
	}

	public Entity ClosestTargetSelection()
	{
		float currentDistance = Mathf.Infinity;

		Entity CurrentTarget = null;

		float dist = 0;
		foreach (var item in m_PotentionalTargets)
		{
			dist = Vector3.Distance(m_BelongTo.transform.position, item.transform.position);
			if (dist < currentDistance)
			{
				currentDistance = dist;
				CurrentTarget = item;
			}
		}

		return CurrentTarget;
	}

	#endregion
}