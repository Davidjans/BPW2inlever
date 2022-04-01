using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sirenix.OdinInspector;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
	#region Instancing
	public static InventoryManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<InventoryManager>(true);
				if (_instance == null)
				{
					_instance = new GameObject("PlayerCharacterManager").AddComponent<InventoryManager>();
				}
			}
			return _instance;
		}
	}

	private static InventoryManager _instance;

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
	public List<BaseGear> m_OwnedGear = new List<BaseGear>();
	public List<string> m_OwnedGearJsons = new List<string>();
	public List<GameObject> m_AssociatedGear = new List<GameObject>();
	private void Start()
	{
		//var jsonFormatter = config.Formatters.JsonFormatter;
		//jsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
		LoadInventory();
		VisualInventoryManager.Instance.SetVisualInventory();
		transform.parent.gameObject.SetActive(false);
	}
	[Button]
	public void LoadInventory()
	{
		string fileName = "PlayerInventory";
		if (SaveManager.CheckJsonExistence(fileName))
		{
			m_OwnedGearJsons = JsonConvert.DeserializeObject<List<string>>(SaveManager.LoadTheJson(fileName));

		}
		else
		{
			ReplaceInstancesWithUniqueInstance();
			m_OwnedGearJsons.Clear();
			foreach (var gear in m_OwnedGear)
			{
				m_OwnedGearJsons.Add(JsonConvert.SerializeObject(gear));
			}
			SaveManager.SaveTheJson(fileName, JsonConvert.SerializeObject(m_OwnedGearJsons));
			m_OwnedGearJsons =  JsonConvert.DeserializeObject<List<string>>(SaveManager.LoadTheJson(fileName));
		}
		CreateGearFromJSon();
		EquipmentManager.Instance.LoadSet();
	}

	[Button]
	public void SaveInventory()
	{
		string fileName = "PlayerInventory";
		ReplaceInstancesWithUniqueInstance();
		m_OwnedGearJsons.Clear();
		foreach (var gear in m_OwnedGear)
		{
			m_OwnedGearJsons.Add(JsonConvert.SerializeObject(gear));
		}
		SaveManager.SaveTheJson(fileName, JsonConvert.SerializeObject(m_OwnedGearJsons));
	}
	[Button]
	public void ReplaceInstancesWithUniqueInstance()
	{
		List<BaseGear> tempList = new List<BaseGear>();
		string gearValuesJson;
		foreach (var gear in m_OwnedGear)
		{
			BaseGear newGear = Instantiate(gear);
			/*gearValuesJson = JsonConvert.SerializeObject(newGear);
			newGear = JSonToSpecificGearType(gearValuesJson);*/
			newGear.LoadOriginalStats();
			newGear.CreateUniqueID();
			// if there is issues with the gearname in the future this is why i hope this temp solution works permanently.
			newGear.LoadSprite();
			tempList.Add(newGear);
		}
		m_OwnedGear = new List<BaseGear>(tempList);
		
	}

	public void CreateGearFromJSon()
	{
		m_OwnedGear.Clear();
		foreach (var gearJson in m_OwnedGearJsons)
		{
			BaseGear gearInstanceStart = BaseGear.CreateInstance<BaseGear>();
			JsonUtility.FromJsonOverwrite(gearJson, gearInstanceStart);
			BaseGear newGear = JSonToSpecificGearType(gearJson);
			newGear.LoadOriginalStats();
			newGear.LoadSprite();
			m_OwnedGear.Add(newGear);
		}
	}

	[Button]
	public void CreateAllAssociatedGear()
	{
		m_AssociatedGear.Clear();
		#if UNITY_EDITOR
				GearOverview.Instance.UpdateGearOverview();
		#endif
		
		for (int i = 0; i < GearOverview.Instance.AllGear.Length; i++)
		{
			if(i >= m_AssociatedGear.Count)
			{
				m_AssociatedGear.Add(null);
			}
		}
		for (int i = 0; i < GearOverview.Instance.AllGear.Length; i++)
		{
			//if(GearOverview.Instance.AllGear[i].m_LinkedGameObject != null)
				m_AssociatedGear[GearOverview.Instance.AllGear[i].m_BaseGearID] = GearOverview.Instance.AllGear[i].m_LinkedGameObject;
		}
	}

	public BaseGear ReturnInstanceOfGearType(BaseGear gear)
	{
		switch (gear.m_TypeName)
		{
			case nameof(BaseWeaponGear):
				Debug.LogError("Sees it as weapon");
				gear = BaseWeaponGear.CreateInstance<BaseWeaponGear>();
				break;
			case nameof(MeleeWeaponGear):
				gear = MeleeWeaponGear.CreateInstance<MeleeWeaponGear>();
				break;
			case nameof(RangedWeaponGear):
				gear = RangedWeaponGear.CreateInstance<RangedWeaponGear>();
				break;
			case nameof(BaseArmor):
				gear = BaseArmor.CreateInstance<BaseArmor>();
				break;
			default:
				gear = BaseGear.CreateInstance<BaseGear>();
				break;
		}
		return gear;
	}
	
	public BaseGear JSonToSpecificGearType(string gearJson)
	{
		BaseGear gear = BaseGear.CreateInstance<BaseGear>();
		gear = JsonConvert.DeserializeObject<BaseGear>(gearJson);
		switch (gear.m_TypeName)
		{
			case nameof(BaseWeaponGear):
				gear = JsonConvert.DeserializeObject<BaseWeaponGear>(gearJson);
				break;
			case nameof(MeleeWeaponGear):
				gear = JsonConvert.DeserializeObject<MeleeWeaponGear>(gearJson);
				break;
			case nameof(RangedWeaponGear):
				gear = JsonConvert.DeserializeObject<RangedWeaponGear>(gearJson);
				break;
			case nameof(BaseArmor):
				gear = JsonConvert.DeserializeObject<BaseArmor>(gearJson);
				break;
			default:
				gear = JsonConvert.DeserializeObject<BaseGear>(gearJson);
				break;
		}
		return gear;
	}

	public void AddGearToInventory(BaseGear gearToAdd)
	{
		m_OwnedGear.Add(gearToAdd);
		SaveInventory();
		VisualInventoryManager.Instance.SetVisualInventory();
	}

	public void OnDisable()
	{
		SaveInventory();
	}
}
