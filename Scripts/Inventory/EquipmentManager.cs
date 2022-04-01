using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class EquipmentManager : SerializedMonoBehaviour
{
	#region Instancing
	public static EquipmentManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<EquipmentManager>(true);
				if (_instance == null)
				{
					_instance = new GameObject("EquipmentManager").AddComponent<EquipmentManager>();
				}
			}
			return _instance;
		}
	}

	private static EquipmentManager _instance;

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
	[OdinSerialize]
	public EquipmentSet m_PlayerEquipment = new EquipmentSet();
	public List<InventorySlot> m_EquipmentSlots = new List<InventorySlot>();
	public bool m_DoneFirstLoad = false;
	private void Start()
	{
		//LoadSet();
	}

	[Button]
	public async void LoadSet()
	{
		await Task.Delay(500);
		string fileName = "PlayerLoadouts";
		if (SaveManager.CheckJsonExistence(fileName))
		{
			m_PlayerEquipment = JsonConvert.DeserializeObject<EquipmentSet>(SaveManager.LoadTheJson(fileName));
		}
		else
		{
			SaveLoadouts();
			m_PlayerEquipment = JsonConvert.DeserializeObject<EquipmentSet>(SaveManager.LoadTheJson(fileName));
		}
		m_PlayerEquipment.OnLoadEquipmentSet();
		SetVisualEquipment();
		m_DoneFirstLoad = true;
	}
	[Button]
	private void SaveLoadouts()
	{
		string fileName = "PlayerLoadouts";
		SaveManager.SaveTheJson(fileName, JsonConvert.SerializeObject(m_PlayerEquipment));
		SetVisualEquipment();
	}

	private void OnDisable()
	{
		if(m_DoneFirstLoad)
			SaveLoadouts();
	}
	
	private void SetVisualEquipment()
	{
		foreach (var slot  in m_EquipmentSlots)
		{
			if (m_PlayerEquipment.m_LoadOutSlotsByGear.ContainsKey(slot.m_SlotType) &&
			    m_PlayerEquipment.m_LoadOutSlotsByGear[slot.m_SlotType] != null)
			{
				slot.LoadItem(m_PlayerEquipment.m_LoadOutSlotsByGear[slot.m_SlotType]);
			}
				
		}
	}
}