using System.Collections;
using System.Collections.Generic;
using EVN.Entities;
using EVN.Movement;
using Sirenix.OdinInspector;
using StackedBeans.Utils;
using UnityEngine;

namespace EVN.Entities
{
	public class Player : Entity
	{
		[FoldoutGroup("ExplorationSettings")] [SerializeField]
		private List<MonoBehaviour> m_TurnOnOnEnterExploration;

		[FoldoutGroup("ExplorationSettings")] [SerializeField]
		private List<MonoBehaviour> m_TurnOffOnEnterExploration;

		[FoldoutGroup("ExplorationSettings")] [SerializeField]
		private List<MonoBehaviour> m_TurnOnOnLeaveExploration;

		[FoldoutGroup("ExplorationSettings")] [SerializeField]
		private List<MonoBehaviour> m_TurnOffOnLeaveExploration;

		[FoldoutGroup("Additional Settings")] public bool m_IsMainCharacter;

		public EquipmentSet equipment;

		// Start is called before the first frame update
		protected override void Start()
		{
			base.Start();

			equipment = EquipmentManager.Instance.m_PlayerEquipment;
			SetEquipmentbonuses();
			OnEnterExploration();
		}

		public override void OnBecomeMyTurn()
		{
			base.OnBecomeMyTurn();
			Debug.LogError("player turn");
			UIManager.Instance.m_TargetInfo.gameObject.SetActive(false);
			PathfindingManager.Instance.OnMouseOver += PlayerCellOver;
		}

		public override void OnNoLongerMyTurn()
		{
			base.OnNoLongerMyTurn();
			PathfindingManager.Instance.OnMouseOver -= PlayerCellOver;
		}

		public override void ChangeAPByValue(float value)
		{
			base.ChangeAPByValue(value);
			UIManager.Instance.ChangeCurrentAPUI(this);
		}

		public void PlayerCellOver(MovementCell cell, Vector2Int xYPos)
		{
			if (UIManager.Instance.m_HoveringOverUI)
				return;
			if (m_MyTurn && !m_Movement.m_CurrentlyMoving)
			{
				if (cell.m_EntityOnCell != null && m_SelectedAbility != -1)
				{
					if(UIManager.Instance.m_TargetInfo.m_TargetEntity != cell.m_EntityOnCell)
						UIManager.Instance.m_TargetInfo.SetTargets(this,cell.m_EntityOnCell);
					
					if (Input.GetMouseButtonDown(0))
					{
						if (cell.m_EntityOnCell.m_EntityFaction != m_EntityFaction)
						{
							CheckAbilityAP();
						}
					}
				}
				else
				{
					UIManager.Instance.m_TargetInfo.gameObject.SetActive(false);
					PathfindingManager.Instance.FindPathWorldPos(
						PathfindingManager.Instance.m_CurrentMovingCharacter.transform.position,
						PathfindingManager.Instance.m_Grid.m_Grid[(xYPos.x, xYPos.y)].GetCellCenter());
					PathfindingManager.Instance.VisualizeCurrentPath();
					if (Input.GetMouseButtonDown(0))
					{
						PathfindingManager.Instance.MoveEntity();
					}
				}
			}
		}

		private void OnExitExploration()
		{
			foreach (var item in m_TurnOffOnLeaveExploration)
			{
				item.enabled = false;
			}

			foreach (var item in m_TurnOnOnLeaveExploration)
			{
				item.enabled = true;
			}
		}

		private void OnEnterExploration()
		{
			foreach (var item in m_TurnOffOnEnterExploration)
			{
				item.enabled = false;
			}

			foreach (var item in m_TurnOnOnEnterExploration)
			{
				item.enabled = true;
			}
		}

		protected override void UseAbility()
		{
			base.UseAbility();
			PathfindingManager.Instance.m_CurrentlyHoveringOver.m_EntityOnCell.TakeHit(
				CalculateToHitOnSelectedAbility(), CalculateDamageOnSelectedAbility());
		}

		public void SetEquipmentbonuses()
		{
			m_AttributeDictionary["AC"].m_CurrentValue = m_AttributeDictionary["AC"].m_CurrentMaxValue;
			foreach (var KVP in equipment.m_LoadOutSlotsByGear)
			{
				if (KVP.Key == EquipmentSlot.Inventory)
					continue;
				if (KVP.Key is EquipmentSlot.LWeapon or EquipmentSlot.RWeapon)
				{
				}
				else
				{
					//Debug.LogError("Stuck here");
					BaseArmor armor = KVP.Value as BaseArmor;
					if (armor != null)
					{
						//Debug.LogError("Should have changed ac Before: " + m_AttributeDictionary["AC"].m_CurrentValue);
						m_AttributeDictionary["AC"].m_CurrentValue += armor.m_ArmorValue;
						//Debug.LogError("Should have changed ac AFter: " + m_AttributeDictionary["AC"].m_CurrentValue);
					}
				}
			}
		}
	}
}