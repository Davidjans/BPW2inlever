using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using EVN.Movement;
using GameCreator.Runtime.Stats.UnityUI;
using EVN.Entities;
using UnityEngine;
using Sirenix.OdinInspector;
public class Enemy : Entity
{

	[FoldoutGroup("AI")]
	public AIOffense m_Offense;

	public Entity m_AttackTarget;

	protected override void Start()
	{
		base.Start();
		m_Offense.m_BelongTo = this;
	}
	
	public override void OnBecomeMyTurn()
	{
		base.OnBecomeMyTurn();
		m_AttackTarget = m_Offense.SelectTarget();
		SelectAbilityToUse();
		if (!CheckWithinAbilityRange(m_AttackTarget.transform))
		{
			MoveWithingAttackRange();
		}
		else
		{
			CheckAbilityAP();
		}
		Debug.LogError("Enemy turn");
	}

	public void SelectAbilityToUse()
	{
		if(m_SelectedAbility != -1)
			SetAbilityValuesDeselect();
		m_SelectedAbility =  (int)Mathf.Round(Random.Range((float)1, (float)m_Abilities.Count)) -1;
		SetAbilityValuesSelect();
		UIManager.Instance.m_CurrentTurnInfo.SetTargetInfo();
	}
	
	public virtual void ChangeAPByValue(float value)
	{
		base.ChangeAPByValue(value);
		if(m_AttributeDictionary["Action"].m_CurrentValue <= 0)
		{
			TurnManager.Instance.NextTurn();
		}
	}
	
	

	public void MoveWithingAttackRange()
	{
		PathfindingManager.Instance.m_CurrentMovingCharacter = m_Movement;
		Vector2Int xYPos = PathfindingManager.Instance.m_Grid.GetXYBasedOnWorldPos(m_AttackTarget.transform.position);
		PathfindingManager.Instance.FindPathWorldPos(PathfindingManager.Instance.m_CurrentMovingCharacter.transform.position,
			PathfindingManager.Instance.m_Grid.m_Grid[(xYPos.x, xYPos.y)].GetCellCenter());
		PathfindingManager.Instance.MoveEntity();
	}

	protected async override void UseAbility()
	{
		base.UseAbility();
		Debug.LogError("used ability");
		m_AttackTarget.TakeHit(CalculateToHitOnSelectedAbility(),CalculateDamageOnSelectedAbility());
		await Task.Delay(2000);
		CheckAbilityAP();
	}
	
	

	protected override void OnEnoughAPForAbility()
	{
		base.OnEnoughAPForAbility();
		
	}
	
	protected override void OnNotEnoughAPForAbility()
	{
		if(TurnManager.Instance.CurrentTurn == this)
			TurnManager.Instance.NextTurn();
	}
}
