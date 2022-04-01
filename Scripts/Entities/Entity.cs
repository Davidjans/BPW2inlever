using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using EVN.Movement;
using EVN.Stats;
using Pathfinding;
using Pathfinding.Examples;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Attribute = EVN.Stats.Attribute;

namespace EVN.Entities
{
	public class Entity : SerializedMonoBehaviour
	{
		[FoldoutGroup("Ranged Attacks")] public Transform m_RangedAbilityOrigin;

		[FoldoutGroup("Ranged Attacks")] public LineRenderer m_RangedAbilityLineRenderer;

		[FoldoutGroup("Stats")] public List<Attribute> m_Attributes;
		[FoldoutGroup("Stats")] public List<Stat> m_Stats;

		[FoldoutGroup("Stats")] [HideInEditorMode]
		public Dictionary<string, Attribute> m_AttributeDictionary;

		[FoldoutGroup("Stats")] [HideInEditorMode]
		public Dictionary<string, Stat> m_StatDictionary;

		[FoldoutGroup("Stats")] public float m_Initiative;

		[FoldoutGroup("Visual")] public Sprite m_ImageRepresentation;

		[FoldoutGroup("Debug Values")] [HideInEditorMode]
		public bool m_MyTurn = false;

		[FoldoutGroup("Debug Values")] [HideInEditorMode]
		public bool m_UsedFreeMovement = false;

		[FoldoutGroup("CombatSettings")] [SerializeField]
		private List<MonoBehaviour> m_TurnOnOnEnterTurn;

		[FoldoutGroup("CombatSettings")] [SerializeField]
		private List<MonoBehaviour> m_TurnOffOnEnterTurn;

		[FoldoutGroup("CombatSettings")] [SerializeField]
		private List<MonoBehaviour> m_TurnOffOnEndTurn;

		[FoldoutGroup("CombatSettings")] [SerializeField]
		private List<MonoBehaviour> m_TurnOnOnEndTurn;

		[FoldoutGroup("CombatSettings")] public List<Ability> m_Abilities = new List<Ability>();

		[FoldoutGroup("CombatSettings")] [HideInEditorMode]
		public int m_SelectedAbility = -1;

		public Faction m_EntityFaction;

		[HideInEditorMode] public EntityMovement m_Movement;
		[HideInEditorMode] public MovementCell m_OnTile;
		public GameObject m_ShieldIcon;
		public GameObject m_HpIcon;
		public TextMeshPro m_DamageTakenText;

		private void Awake()
		{
			for (int i = 0; i < m_Abilities.Count; i++)
			{
				m_Abilities[i].m_BelongTo = this;
				m_Abilities[i].m_AbilityNumberOnEntity = i;
			}

			SetToDictionary();
		}

		protected virtual void Start()
		{
			m_Movement = GetComponent<EntityMovement>();
			SetOnTile();
		}

		public virtual void ChangeAPByValue(float value)
		{
			//Debug.LogError( GetType() + "   from:  " + m_AttributeDictionary["Action"].m_CurrentValue);
			//Debug.LogError( GetType() + "   valueToChangeBy:  " + value);
			m_AttributeDictionary["Action"].m_CurrentValue = Mathf.Clamp(
				m_AttributeDictionary["Action"].m_CurrentValue + value, -50,
				m_AttributeDictionary["Action"].m_CurrentMaxValue);
			//Debug.LogError( GetType() + "   To:  " + m_AttributeDictionary["Action"].m_CurrentValue);
		}


		public virtual void OnNoLongerMyTurn()
		{
			m_UsedFreeMovement = false;
			m_MyTurn = false;
			foreach (var item in m_TurnOffOnEndTurn)
			{
				item.enabled = false;
			}

			foreach (var item in m_TurnOnOnEndTurn)
			{
				item.enabled = true;
			}

			m_SelectedAbility = -1;
			UIManager.Instance.SelectAbilityUI(-1);
			SetOnTile();
		}

		public virtual void OnBecomeMyTurn()
		{
			//TurnBasedManager.Instance.SetUnitTurn(this);
			PathfindingManager.Instance.m_CurrentMovingCharacter = m_Movement;
			m_MyTurn = true;
			m_UsedFreeMovement = false;
			VisualizeManager.Instance.m_CurrentStart = transform;
			ChangeAPByValue(m_AttributeDictionary["Action"].m_CurrentMaxValue);
			foreach (var item in m_TurnOnOnEnterTurn)
			{
				item.enabled = false;
			}

			foreach (var item in m_TurnOffOnEnterTurn)
			{
				item.enabled = true;
			}

			m_SelectedAbility = -1;
			UIManager.Instance.SelectAbilityUI(-1);
			SetOnTile();
			UIManager.Instance.m_CurrentTurnInfo.SetTargets(null,this);
			//GetComponent<AIDestinationSetter>().target.position = transform.position;
		}

		public void SetAbilityValuesDeselect()
		{
			Ability ability = m_Abilities[m_SelectedAbility];
			if (ability.m_AbilityType == AbilityType.Ranged)
			{
				SetRangedAbilityValuesEnd(ability);
			}
		}


		public void SetAbilityValuesSelect()
		{
			Ability ability = m_Abilities[m_SelectedAbility];
			if (ability.m_AbilityType == AbilityType.Ranged)
			{
				SetRangedAbilityValuesStart(ability);
			}
		}

		private void SetRangedAbilityValuesEnd(Ability ability)
		{
			RangedAttack rangedAbility = (RangedAttack) ability;
			rangedAbility.ManualSetValuesOnEnd();
		}

		private void SetRangedAbilityValuesStart(Ability ability)
		{
			RangedAttack rangedAbility = (RangedAttack) ability;
			if (m_RangedAbilityOrigin != null)
			{
				rangedAbility.ManualSetValuesOnStart(m_RangedAbilityOrigin, m_RangedAbilityLineRenderer);
			}
		}

		public void OnCombatStart()
		{
		}

		public void SetOnTile()
		{
			if (m_OnTile != null)
			{
				m_OnTile.m_EntityOnCell = null;
			}
			m_OnTile =
				(MovementCell) PathfindingManager.Instance.m_Grid.GetCellOnPosition<MovementCell>(transform.position);
			m_OnTile.m_EntityOnCell = this;
		}

		public async void TakeHit(float toHit, float damage)
		{
			if (toHit >= m_AttributeDictionary["AC"].m_CurrentValue)
			{
				m_AttributeDictionary["Health"].m_CurrentValue -= damage;
				m_HpIcon.SetActive(true);
				m_DamageTakenText.text = Mathf.Round(damage).ToString();
				UIManager.Instance.m_TargetInfo.SetTargetInfo();
				await Task.Delay(2000);
				m_HpIcon.SetActive(false);
				if (m_AttributeDictionary["Health"].m_CurrentValue <= 0)
				{
					OnDeath();
				}
			}
			else
			{
				m_ShieldIcon.SetActive(true);
				await Task.Delay(2000);
				m_ShieldIcon.SetActive(false);
			}
		}

		private void SetToDictionary()
		{
			m_AttributeDictionary = new Dictionary<string, Attribute>();
			m_StatDictionary = new Dictionary<string, Stat>();
			foreach (var stat in m_Stats)
			{
				Stat newStat = Instantiate(stat);
				newStat.m_CurrentValue = newStat.BaseValue;
				m_StatDictionary.Add(stat.ID, newStat);
			}

			foreach (var attribute in m_Attributes)
			{
				Attribute newAttribute = Instantiate(attribute);
				newAttribute.m_StatToPerformCalcWith = m_StatDictionary[newAttribute.m_StatToPerformCalcWith.ID];
				newAttribute.m_CurrentMaxValue = newAttribute.BaseValue +
				                                 (newAttribute.m_StatCalcImpact *
				                                  newAttribute.m_StatToPerformCalcWith.m_CurrentValue);
				if (newAttribute.m_RoundResult)
					newAttribute.m_CurrentMaxValue = Mathf.Floor(newAttribute.m_CurrentMaxValue);
				newAttribute.m_CurrentValue = newAttribute.m_CurrentMaxValue;
				m_AttributeDictionary.Add(newAttribute.ID, newAttribute);
			}
		}

		public virtual void OnDeath()
		{
			CombatManager.Instance.m_EntitiesInCombat.Remove(this);
			TurnManager.Instance.m_EntitiesByInitiative.Remove(this);
			CombatManager.Instance.CheckCombatOver();
		}
		public virtual bool CheckAbilityAP()
		{
			if (m_Abilities[m_SelectedAbility].APCost <= m_AttributeDictionary["Action"].m_CurrentValue)
			{
				OnEnoughAPForAbility();
				return true;
			}
			else
			{
				Debug.LogError("Not enough AP");
				OnNotEnoughAPForAbility();
				return false;
			}
		}

		protected virtual void UseAbility()
		{
			ChangeAPByValue(-m_Abilities[m_SelectedAbility].APCost);
		}

		public virtual float CalculateToHitOnSelectedAbility()
		{
			float ToHit = UnityEngine.Random.Range(0, 21) + GetToHitBonus();
			
			
			return  ToHit;
		}

		public float GetToHitBonus()
		{
			float ToHit = 0;
			if (m_SelectedAbility != -1)
			{
				ToHit += m_StatDictionary[m_Abilities[m_SelectedAbility].m_StatToScaleOff.ID].m_CurrentValue *
				         m_Abilities[m_SelectedAbility].m_BonusToHitPerStatPoint;
			}
			return ToHit;
		}
		
		public virtual float CalculateDamageOnSelectedAbility()
		{
			float damage = UnityEngine.Random.Range(m_Abilities[m_SelectedAbility].m_MinMaxDamage.x,m_Abilities[m_SelectedAbility].m_MinMaxDamage.y);
			damage += m_StatDictionary[m_Abilities[m_SelectedAbility].m_StatToScaleOff.ID].m_CurrentValue *
			          m_Abilities[m_SelectedAbility].m_BonusDamagePerStatPoint;
			return  damage;
		}
		protected virtual void OnEnoughAPForAbility()
		{
			UseAbility();
		}
		protected virtual void OnNotEnoughAPForAbility()
		{
			
		}
		
		public bool CheckWithinAbilityRange(Transform target)
		{
			float distance = Vector3.Distance(target.position, transform.position);
			if (distance < m_Abilities[m_SelectedAbility].m_MaxRange)
				return true;
			else
				return false;
		}
		
		public virtual void OnEnable()
		{
			EntityManager.Instance.AddEntity(this);
		}

		public virtual void OnDisable()
		{
			EntityManager.Instance.RemoveEntity(this);
		}
		
		
	}
}