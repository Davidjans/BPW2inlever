using System;
using System.Collections;
using System.Collections.Generic;
using EVN.Entities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Base", menuName = "StackedBeans/AI/Movement/Base", order = -99)]
public class AIMovement : UnityEngine.ScriptableObject
{
	[HideInEditorMode] public Entity m_BelongTo;

	public float m_CoverConsideration = 1;


	public virtual void CalculateDestination()
	{

	}
}