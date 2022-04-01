using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using EVN.Entities;

public class CombatRichAI : RichAI
{
	[SerializeField] private Entity m_Entity;

	protected override void Start()
	{
		base.Start();
		if (m_Entity == null) 
		{
			m_Entity = GetComponent<Entity>();
		}
		destination = transform.position;
	}

	protected override void Update()
	{
		if (!m_Entity.m_MyTurn)
		{
			return;
		}
		base.Update();
	}
}
