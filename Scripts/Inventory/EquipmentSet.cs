using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using System;

[Serializable]
public class EquipmentSet
{
	public Dictionary<EquipmentSlot, float> m_LoadoutSlotsByID = new Dictionary<EquipmentSlot, float>();

	[JsonIgnore]
	public Dictionary<EquipmentSlot, BaseGear> m_LoadOutSlotsByGear = new Dictionary<EquipmentSlot, BaseGear>();

	public List<float> m_BackpackSlotsUniqueID;

	private void Awake()
	{
		if (m_LoadoutSlotsByID == null || m_LoadoutSlotsByID.Count == 0)
		{
			OnCreate();
		}
	}

	private void OnCreate()
	{
		m_LoadoutSlotsByID = new Dictionary<EquipmentSlot, float>();
		m_LoadOutSlotsByGear = new Dictionary<EquipmentSlot, BaseGear>();
		foreach (EquipmentSlot item in Enum.GetValues(typeof(EquipmentSlot)))
		{
			if (!m_LoadoutSlotsByID.ContainsKey(item))
			{
				m_LoadoutSlotsByID.Add(item, 0);
				m_LoadOutSlotsByGear.Add(item, null);
			}
		}
	}

	[Button]
	public void ChangeSlot(EquipmentSlot slotToChange, BaseGear gearInSlot, int backpackSlotReplace = 0)
	{
		if (slotToChange != EquipmentSlot.Inventory && gearInSlot != null)
		{
			if (!m_LoadoutSlotsByID.ContainsKey(slotToChange))
			{
				m_LoadoutSlotsByID.Add(slotToChange, 0);
				m_LoadOutSlotsByGear.Add(slotToChange, null);
			}

			m_LoadoutSlotsByID[slotToChange] = gearInSlot.m_UniqueGearID;
			m_LoadOutSlotsByGear[slotToChange] = gearInSlot;
		}
	}

	public void OnLoadEquipmentSet()
	{
		if (m_LoadoutSlotsByID == null || m_LoadoutSlotsByID.Count == 0)
		{
			OnCreate();
		}

		foreach (var id in m_LoadoutSlotsByID)
		{
			if(!m_LoadOutSlotsByGear.ContainsKey(id.Key))
				m_LoadOutSlotsByGear.Add(id.Key, null);
			foreach (var gear in InventoryManager.Instance.m_OwnedGear)
			{
				if (gear.m_UniqueGearID == id.Value)
				{
					m_LoadOutSlotsByGear[id.Key] = gear;
					gear.m_InLoadout = true;
					break;
				}
			}
		}
	}

	[Button]
	public void DebugAddToLoadout(EquipmentSlot slotToChange, uint gearID, int backpackSlotReplace = 0)
	{
		if (slotToChange != EquipmentSlot.Inventory)
		{
			if (!m_LoadoutSlotsByID.ContainsKey(slotToChange))
			{
				m_LoadoutSlotsByID.Add(slotToChange, 0);
			}

			m_LoadoutSlotsByID[slotToChange] = gearID;
		}
	}
}