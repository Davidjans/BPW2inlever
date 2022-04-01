using System;
using System.Collections;
using System.Collections.Generic;
using EVN.Entities;
using EVN.Stats;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = " Base", menuName = "StackedBeans/Abilities/Base", order = -99)]
public class Ability : ScriptableObject
{
	[HideInEditorMode] public Entity m_BelongTo;

	public float APCost;
	public Sprite m_AbilityImage;
	public int m_AbilityNumberOnEntity = 0;
	public float m_MinRange = 0;
	public float m_MaxRange = 1.5f;
	public bool m_IsAbilityUnlocked;
	public bool m_IsAbilityBought;
	public Stat m_StatToScaleOff;
	public float m_BonusDamagePerStatPoint = 1;
	public float m_BonusToHitPerStatPoint = 0.5f;
	public Vector2 m_MinMaxDamage = new Vector2(0, 12);
	public AbilityType m_AbilityType = AbilityType.Ranged;


	public virtual void ShowAbilityVisual()
	{
		
	}

	public virtual void ManualSetValuesOnStart()
	{

	}
	public virtual void ManualSetTarget()
	{
	}
	public virtual void ManualSetValuesOnEnd()
	{

	}


	public virtual void AbilityAction()
	{
		Debug.Log("Ability action performed!");
	}
}