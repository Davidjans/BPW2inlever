using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EVN;
using Sirenix.OdinInspector;
using UnityEngine;
using EVN.Entities;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
	#region MyRegion
	public static CombatManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<CombatManager>();
				if (_instance == null)
				{
					_instance = new GameObject("CombatManager").AddComponent<CombatManager>();
				}
			}
			return _instance;
		}
	}
	#endregion
	

	private static CombatManager _instance;

	[FoldoutGroup("Assign Things")]
	public TurnManager m_TurnManager;

	[HideInEditorMode]
	[FoldoutGroup("Debug Values")] public List<Entity> m_EntitiesInCombat;

	

	[FoldoutGroup("Debug Values")] public bool m_InCombat;



	[HideInEditorMode]
	[Button("Start Combat")]
	public void StartCombat()
	{
		m_InCombat = true;
		AddEntitiesToCombat();
		foreach (var entity in m_EntitiesInCombat)
		{
			entity.OnCombatStart();
		}
		
		m_TurnManager.OnCombatStart();
	}

	public void CheckCombatOver()
	{
		bool enemiesAlive = false;
		bool alliesAlive = false;
		foreach (var entity in m_EntitiesInCombat)
		{
			if (entity.m_EntityFaction == Faction.Friendly)
			{
				alliesAlive = true;
			}
			else if (entity.m_EntityFaction == Faction.Hostile)
			{
				enemiesAlive = true;
			}
		}

		if (!enemiesAlive && alliesAlive)
		{ 
			OnCombatEnd(true);
		}
		else if (!alliesAlive && enemiesAlive)
		{
			OnCombatEnd(false);
		}
	}
	public void OnCombatEnd(bool victory)
	{
		if (victory)
		{
			LootChoiceManager.Instance.EnableChoices();
		}
		else
		{
			SceneManager.LoadScene(0);
		}
	}
	
	private void Awake()
	{
		// Destroy any duplicate instances that may have been created
		if (_instance != null && _instance != this)
		{
			Destroy(this);
			return;
		}

		_instance = this;
		if(m_TurnManager == null)
		{
			m_TurnManager = TurnManager.Instance;
		}
	}

	private void AddEntitiesToCombat()
	{
		m_EntitiesInCombat = EntityManager.Instance.m_AliveEntities;
		List<Transform> entityTransforms = new List<Transform>();
		foreach (var entity in m_EntitiesInCombat)
		{
			entityTransforms.Add(entity.transform);
		}
		foreach (var item in FindObjectsOfType<MultipleTargetsCamera>())
		{
			item.SetTargets(entityTransforms);
		}
	}
}
