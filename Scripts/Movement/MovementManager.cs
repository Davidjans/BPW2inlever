using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class MovementManager : MonoBehaviour
{
	public static MovementManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<MovementManager>();
				if (_instance == null)
				{
					_instance = new GameObject("MovementManager").AddComponent<MovementManager>();
				}
			}
			return _instance;
		}
	}

	private static MovementManager _instance;

	[HideInEditorMode]
    [FoldoutGroup("Debug Values")] public float m_CurrentPathLength;

    [HideInEditorMode]
    [FoldoutGroup("Debug Values")] public float m_CurrentPathCost;

    void Awake()
    {
		// Destroy any duplicate instances that may have been created
		if (_instance != null && _instance != this)
		{
			Destroy(this);
			return;
		}

		_instance = this;
	}

	public void SetVisualizationValues(float pathLength, float movementCost)
	{
		m_CurrentPathCost = movementCost;
		m_CurrentPathLength = pathLength;
	}

}