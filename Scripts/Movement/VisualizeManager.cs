using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;


public class VisualizeManager : MonoBehaviour
{
	public static VisualizeManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<VisualizeManager>();
				if (_instance == null)
				{
					_instance = new GameObject("VisualizeManager").AddComponent<VisualizeManager>();
				}
			}
			return _instance;
		}
	}

	private static VisualizeManager _instance;

	private Seeker m_Seeker;

	public Transform m_CurrentStart;

	[HideInEditorMode] public Vector3 m_EndPosition;

	[FoldoutGroup("UI")]
	[SerializeField]
	private Transform m_UITransform;

	[FoldoutGroup("UI")]
	[SerializeField]
	private TextMeshPro m_TextMesh;


	public LineRenderer m_LineRenderer;

	[HideInEditorMode]
	public Path m_CurrentPath;

	[HideInEditorMode]
	public float m_CurrentPathLength = 0;

	public bool m_CurrentlyVisualize = false;

	[HideInEditorMode] 
	public bool m_CurrentlyVisualizing = false;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this);
			return;
		}

		_instance = this;

		m_Seeker = GetComponent<Seeker>();
		m_LineRenderer = GetComponent<LineRenderer>();
		m_LineRenderer.enabled = false;
		SetNewPath();
	}

	private void Update()
	{
		if (m_CurrentlyVisualize && !m_CurrentlyVisualizing)
		{
			VisualizePath();
		}
	}
	
	public void StartVisualization(Vector3 EndPosition)
	{
		m_EndPosition = EndPosition;
		m_CurrentlyVisualize = true;
		enabled = true;
		m_LineRenderer.enabled = true;
	}


	public void VisualizePath()
    {
		SetNewPath();
    }

    private void SetNewPath()
    {
	    m_CurrentlyVisualizing = true;
	    RaycastHit hit;

		if(m_CurrentStart != null)
			m_Seeker.StartPath(m_CurrentStart.position, m_EndPosition, OnPathComplete);
		else
		{
			m_CurrentlyVisualizing = false;
		}
    }

    public void OnPathComplete(Path p)
    {
	    m_CurrentlyVisualizing = false;
		m_CurrentPath = m_Seeker.GetCurrentPath();
		m_UITransform.position = m_EndPosition;
		m_CurrentPathLength = m_CurrentPath.GetTotalLength();
		float movementCost = Mathf.Floor(m_CurrentPathLength / TurnManager.Instance.CurrentTurn.m_AttributeDictionary["Movement"].m_CurrentValue);
		if (TurnManager.Instance.CurrentTurn.m_UsedFreeMovement)
		{
			movementCost += 1;
		}

		MovementManager.Instance.SetVisualizationValues(m_CurrentPathLength, movementCost);
		m_TextMesh.text = m_CurrentPathLength.ToString("F2")+ " M " +  " AP: " + movementCost;
	    var buffer = new List<Vector3>();
	    buffer = m_CurrentPath.vectorPath;
	    m_LineRenderer.positionCount = buffer.Count;
	    m_LineRenderer.SetPositions(buffer.ToArray());
    }
}
