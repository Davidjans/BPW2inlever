using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class DifficultyManager : ScriptableObject
{
	[TabGroup("General Difficulty")]
	[HideInInspector]
	private float placeholder;

	[TabGroup("General Difficulty/Loot")]
	public float m_GoldEarnMultiplyer = 1;

	[TabGroup("General Difficulty/Loot")]
	public float m_ResourceGainMultiplyer = 1;

	[TabGroup("General Difficulty/Loot")]
	public float m_ItemStatMultiplyer = 1;

	[TabGroup("General Difficulty/Loot")]
	public float m_LootDropChanceMultiplyer = 1;

	[TabGroup("General Difficulty/Enemy")]
	public float m_DamageMultiplyer = 1;

	[TabGroup("General Difficulty/Enemy")]
	public float m_HealthMultiplyer = 1;

	[TabGroup("General Difficulty/Enemy")]
	public float m_APMultiplyer = 1;
}
