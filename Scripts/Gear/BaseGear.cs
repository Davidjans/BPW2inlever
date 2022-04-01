using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Gear", menuName = "StackedBeans/Gear/BaseGear", order = 1)]
public class BaseGear : SerializedScriptableObject
{
	public string m_BaseGearName;
	public int m_BaseGearID;
	public uint m_UniqueGearID;
	public int m_GearLevel = 1;
	public string m_TypeName;
	
	[JsonIgnore]
	public bool m_InLoadout = false;
	
	[JsonIgnore]
	public GameObject m_LinkedGameObject;
	
	[JsonIgnore]
	public List<EquipmentSlot> m_AllowedEquipmentSlots = new List<EquipmentSlot>();
	
	[JsonIgnore]
	public Sprite m_GearSprite;
	
	[JsonIgnore] 
	public BaseGear m_OriginalGearLink;

	[JsonIgnore] 
	public InventorySlot m_InInventorySlot;
	
	[HideInEditorMode] public bool m_FirstCreation = true;
	
	[JsonIgnore]
	public Rarity m_Rarity;

	public string m_RarityName;
	
	[MinMaxSlider(0, 500, true)] public Vector2 m_AllocationRange = new Vector2(0, 50);
	[HideInEditorMode] public float m_AllocationPoints;

	private List<bool> m_PurchaseAllows;
	
	
	[Button]
	private void Awake()
	{
		if (m_FirstCreation)
		{
			OnCreate();
		}
	}

	[Button]
	private void SetGearNameToItemName()
	{
		m_BaseGearName = name;
	}
	public void LoadOriginalStats()
	{
		if (m_OriginalGearLink != null)
		{
			Debug.LogError("Already did that");
			return;
		}
		foreach (var gear in GearOverview.Instance.AllGear)
		{
			
			if (gear.m_BaseGearName == m_BaseGearName )
			{
				m_BaseGearID = gear.m_BaseGearID;
				name = m_BaseGearName;
				m_OriginalGearLink = gear;
				m_LinkedGameObject = gear.m_LinkedGameObject;
			}
		}

		if (m_OriginalGearLink == null)
		{
			Debug.LogError("The fuck it didn't find any");
			return;
		}
		m_AllowedEquipmentSlots = m_OriginalGearLink.m_AllowedEquipmentSlots;
	}
	public async void LoadSprite()
	{
		LoadRarity();
		if (m_GearSprite != null) return;
		//Sprite sprite =  await LoadSpriteAsync();
		await LoadSpriteAsync();
		
		m_InInventorySlot?.LoadItemVisual();
			
	}

	private async UniTask<Sprite> LoadSpriteAsync()
	{
		AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(m_BaseGearName + "Icon");
		await handle.Task;
		if(handle.Status == AsyncOperationStatus.Succeeded)
		{
			m_GearSprite = handle.Result;
		}
		return null;
	}
	
	public async void LoadRarity()
	{
		if (m_Rarity != null) return;
		await LoadRarityAsync();
	}

	private async UniTask<Rarity> LoadRarityAsync()
	{
		AsyncOperationHandle<Rarity> handle = Addressables.LoadAssetAsync<Rarity>(m_RarityName);
		await handle.Task;
		if(handle.Status == AsyncOperationStatus.Succeeded)
		{
			m_Rarity = handle.Result;
		}
		return null;
	}
	
	[Button]
	private void OnCreate()
	{
		CreateBaseID();
		m_FirstCreation = false;
		m_TypeName = GetType().Name;
	}
	public void CreateBaseID()
	{
		#if  UNITY_EDITOR
				GearOverview.Instance.UpdateGearOverview();
		#endif
		
		m_BaseGearID = GearOverview.Instance.AllGear.Length;
	}
	public void CreateUniqueID()
	{
		if (m_UniqueGearID == 0)
		{
			m_UniqueGearID = (uint)Random.Range(0, int.MaxValue);
		}
	}

	public virtual void GenerateRandomGear()
	{
		m_PurchaseAllows = new List<bool>();
		SetBaseValues();

		m_PurchaseAllows = AddPurchaseAllows();
		
		m_AllocationPoints = Mathf.RoundToInt(Random.Range(m_AllocationRange.x, m_AllocationRange.y));
		while (m_AllocationPoints > 0)
		{
			int itemToPurchase = Random.Range(0, m_PurchaseAllows.Count);
			if (m_PurchaseAllows[itemToPurchase])
			{
				PurchaseUpgrade(itemToPurchase);
				m_AllocationPoints--;
			}
		}
		SetGearValues();
	}
	
	protected  virtual List<bool> AddPurchaseAllows()
	{
		List<bool> purchaseAllows = new List<bool>();
		/*for (int i = 0; i < Enum.GetValues(typeof(RangedWeaponUpgradeFeature)).Length; i++)
		{
			m_PurchaseAllows.Add(true);
		}*/
		return purchaseAllows;
	}

	protected virtual void PurchaseUpgrade(int thingToUpgrade)
	{
		/*RangedWeaponUpgradeFeature upgradeFeature = (RangedWeaponUpgradeFeature)thingToUpgrade;
		switch (upgradeFeature)
		{
			default:
				throw new ArgumentOutOfRangeException();
		}*/
	}
	protected virtual void SetBaseValues()
	{
	}

	protected virtual void SetGearValues()
	{
	}
}
