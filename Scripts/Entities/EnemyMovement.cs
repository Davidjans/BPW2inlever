using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVN.Movement
{
	public class EnemyMovement : EntityMovement
	{
		public override void GoToNextNode()
		{
			Enemy parentEnemy = m_ParentEntity.GetComponent<Enemy>();
			if (parentEnemy.CheckWithinAbilityRange(parentEnemy.m_AttackTarget.transform))
			{
				Enemy enemy = (Enemy)m_ParentEntity;
				enemy.CheckAbilityAP();
				EndMovement();
				return;
			}
			base.GoToNextNode();
		}

		public override void EndMovement()
		{
			base.EndMovement();
			if (m_ParentEntity.m_AttributeDictionary["Action"].m_CurrentValue <= 0 && TurnManager.Instance.CurrentTurn == m_ParentEntity)
			{
				TurnManager.Instance.NextTurn();
			}
			
		}
	}
}

