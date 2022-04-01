using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootChoiceManager : MonoBehaviour
{
	#region Instancing
	public static LootChoiceManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<LootChoiceManager>(true);
				if (_instance == null)
				{
					_instance = new GameObject("LootChoiceManager").AddComponent<LootChoiceManager>();
				}
			}
			return _instance;
		}
	}

	private static LootChoiceManager _instance;

	void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		if (_instance != this)
		{
			Destroy(this.gameObject);
			return;
		}
	}
	#endregion
	public List<ItemChoice> m_ItemChoice = new List<ItemChoice>();
	public List<Rarity> m_PossibleRariteies = new List<Rarity>();
	private void Start()
	{
		#if UNITY_EDITOR
			GearOverview.Instance.UpdateGearOverview();
		#endif
		foreach (var itemChoice in m_ItemChoice)
		{
			BaseGear gear =
				Instantiate(GearOverview.Instance.AllGear[Random.Range(0, GearOverview.Instance.AllGear.Length)]);
			gear.m_Rarity = m_PossibleRariteies[ Random.Range(0,m_PossibleRariteies.Count)];
			gear.m_RarityName = gear.m_Rarity.name;
			//gear.GenerateRandomGear();
			itemChoice.ImplementItem(gear);
		}
	}

	public void EnableChoices()
	{
		transform.parent.gameObject.SetActive(true);
	}
}