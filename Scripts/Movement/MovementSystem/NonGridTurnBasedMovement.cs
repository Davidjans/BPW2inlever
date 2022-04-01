using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EVN.Entities;
using Pathfinding;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.EventSystems;
using Pathfinding.Examples;
	/// <summary>Helper script in the example scene 'Turn Based'</summary>
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_examples_1_1_turn_based_manager.php")]
public class NonGridTurnBasedMovement : MonoBehaviour
{

	/// <summary>
	/// Instance of our Singleton
	/// </summary>
	public static NonGridTurnBasedMovement Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<NonGridTurnBasedMovement>();
				if (_instance == null)
				{
					_instance = new GameObject("TurnBasedManager").AddComponent<NonGridTurnBasedMovement>();
				}
			}
			return _instance;
		}
	}
	private static NonGridTurnBasedMovement _instance;

	Entity selected;

	public float movementSpeed;
	public LayerMask layerMask;

	[HideInEditorMode]
	[SerializeField] List<GameObject> possibleMoves = new List<GameObject>();
	EventSystem eventSystem;

	[HideInEditorMode]
	public TurnState turnState = TurnState.SelectUnit;

	[HideInEditorMode]
	public SelectionState selectState = SelectionState.UnitMove;

	[HideInEditorMode]
	[FoldoutGroup("Debug Values")] public bool m_CurrentlyMoving;

	private Astar3DButton m_LastButton;


	public Transform m_TestTarget;
	public Transform m_TestOrigin;

	public enum TurnState
	{
		SelectUnit,
		SelectTarget,
		Move
	}

	public enum SelectionState
	{
		UnitMove,
		UnitAbility
	}
	void Awake()
	{
		eventSystem = FindObjectOfType<EventSystem>();
		// Destroy any duplicate instances that may have been created
		if (_instance != null && _instance != this)
		{
			Destroy(this);
			return;
		}

		_instance = this;
	}

	void Update()
	{
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		CheckTargetingState();

		// Ignore any input while the mouse is over a UI element
		if (eventSystem.IsPointerOverGameObject())
		{
			return;
		}

			

		if (!CombatManager.Instance.m_InCombat)
		{
			if ((turnState == TurnState.SelectUnit || turnState == TurnState.SelectTarget))
			{
				if (Input.GetKeyDown(KeyCode.Mouse0))
				{
					var unitUnderMouse = GetByRay<Entity>(ray);

					if (unitUnderMouse != null)
					{

						Select(unitUnderMouse);
						//DestroyPossibleMoves();
						//GeneratePossibleMoves(selected);
						VisualizeManager.Instance.m_CurrentStart = selected.transform;
						VisualizeManager.Instance.m_LineRenderer.enabled = false;
						VisualizeManager.Instance.m_CurrentlyVisualizing = false;

						turnState = TurnState.SelectTarget;
					}
				}
			}
		}
		else
		{
			if (turnState == TurnState.SelectUnit)
			{
				Select(TurnManager.Instance.m_EntitiesByInitiative[0]);
				//DestroyPossibleMoves();
				//GeneratePossibleMoves(selected);

			}
			else if (turnState == TurnState.SelectTarget)
			{
				if (selectState == SelectionState.UnitMove)
				{
					HandleButtonUnderRay(ray);
				}
				else if (selectState == SelectionState.UnitAbility)
				{
					if (Input.GetMouseButtonDown(0))
					{
						DoAbilityAction();
					}
					else
					{
						DoAbilityVisual();
					}
				}
			}
		}

	}

	public void SetUnitTurn(Entity unit)
	{
		Select(unit);
		//DestroyPossibleMoves();
		//GeneratePossibleMoves(selected);
		VisualizeManager.Instance.m_CurrentStart = selected.transform;
		VisualizeManager.Instance.m_LineRenderer.enabled = false;
		VisualizeManager.Instance.m_CurrentlyVisualizing = false;

		turnState = TurnState.SelectTarget;
	}

	// TODO: Move to separate class
	void HandleButtonUnderRay(Ray ray)
	{

		RaycastHit hit;
			
		if(Physics.Raycast(ray, out hit, float.PositiveInfinity, layerMask))
		{
			VisualizeManager.Instance.StartVisualization(hit.point);
		}
	}

	T GetByRay<T>(Ray ray) where T : class
	{
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layerMask))
		{
			return hit.transform.GetComponentInParent<T>();
		}
		return null;
	}

	void Select(Entity unit)
	{
		selected = unit;
		turnState = TurnState.SelectTarget;
	}

	IEnumerator MoveToNode(Entity unit, GraphNode node)
	{
		var path = ABPath.Construct(unit.transform.position, (Vector3)node.position);

		//path.traversalProvider = unit.traversalProvider;

		// Schedule the path for calculation
		AstarPath.StartPath(path);

		// Wait for the path calculation to complete
		yield return StartCoroutine(path.WaitForPath());

		if (path.error)
		{
			// Not obvious what to do here, but show the possible moves again
			// and let the player choose another target node
			// Likely a node was blocked between the possible moves being
			// generated and the player choosing which node to move to
			Debug.LogError("Path failed:\n" + path.errorLog);
			turnState = TurnState.SelectTarget;
			//GeneratePossibleMoves(selected);
			yield break;
		}

		// Set the target node so other scripts know which
		// node is the end point in the path
		TurnBasedAI turnBasedAI = unit.GetComponentInChildren<TurnBasedAI>();
		turnBasedAI.targetNode = path.path[path.path.Count - 1];

		yield return StartCoroutine(MoveAlongPath(unit, path, movementSpeed));

		//unit.blocker.BlockAtCurrentPosition();

		if (unit.m_AttributeDictionary["Actions"].m_CurrentValue <= 0)
		{
			TurnManager.Instance.NextTurn();
		}

		// Select a new unit to move
		turnState = TurnState.SelectUnit;
	}

	/// <summary>Interpolates the unit along the path</summary>
	static IEnumerator MoveAlongPath(Entity unit, ABPath path, float speed)
	{
		if (path.error || path.vectorPath.Count == 0)
			throw new System.ArgumentException("Cannot follow an empty path");

		// Very simple movement, just interpolate using a catmull rom spline
		float distanceAlongSegment = 0;
		for (int i = 0; i < path.vectorPath.Count - 1; i++)
		{
			var p0 = path.vectorPath[Mathf.Max(i - 1, 0)];
			// Start of current segment
			var p1 = path.vectorPath[i];
			// End of current segment
			var p2 = path.vectorPath[i + 1];
			var p3 = path.vectorPath[Mathf.Min(i + 2, path.vectorPath.Count - 1)];

			var segmentLength = Vector3.Distance(p1, p2);

			while (distanceAlongSegment < segmentLength)
			{
				var interpolatedPoint = AstarSplines.CatmullRom(p0, p1, p2, p3, distanceAlongSegment / segmentLength);
				unit.transform.position = interpolatedPoint;
				yield return null;
				distanceAlongSegment += Time.deltaTime * speed;
			}

			distanceAlongSegment -= segmentLength;
		}

		unit.transform.position = path.vectorPath[path.vectorPath.Count - 1];
	}


	private void ShowCurrentAbilityVisual()
	{

	}

	private void CheckTargetingState()
	{
		if (turnState == TurnState.SelectTarget)
		{
			if (TurnManager.Instance.CurrentTurn.m_SelectedAbility == -1)
			{
				selectState = SelectionState.UnitMove;
				//if (!m_MovesGenerated)
				//{
					//GeneratePossibleMoves(selected);
				//}
			}
			else
			{
				selectState = SelectionState.UnitAbility;
				//if (m_MovesGenerated)
				//{
				//	DestroyPossibleMoves();
				//}
			}
		}
	}

	private void DoAbilityVisual()
	{
		AbilityType abilityType = TurnManager.Instance.CurrentTurn
			.m_Abilities[TurnManager.Instance.CurrentTurn.m_SelectedAbility].m_AbilityType;
		if (abilityType == AbilityType.Ranged)
		{
			ShowRangedAbilityVisual();
		}
		else if (abilityType == AbilityType.Melee)
		{
			ShowMeleeAbilityVisual();
		}
		else if (abilityType == AbilityType.AOE)
		{
			ShowAOEAbilityVisual();
		}
		else if (abilityType == AbilityType.ETC)
		{
			ShowETCAbilityVisual();
		}
	}

	private void ShowRangedAbilityVisual()
	{
		RangedAttack attack = (RangedAttack)TurnManager.Instance.CurrentTurn.m_Abilities[TurnManager.Instance.CurrentTurn.m_SelectedAbility];



		attack.ShowAbilityVisual();
	}

	private void ShowMeleeAbilityVisual()
	{
	}
	private void ShowAOEAbilityVisual()
	{
	}
	private void ShowETCAbilityVisual()
	{
	}

	private void DoAbilityAction()
	{
		AbilityType abilityType = TurnManager.Instance.CurrentTurn
			.m_Abilities[TurnManager.Instance.CurrentTurn.m_SelectedAbility].m_AbilityType;
		if (abilityType == AbilityType.Ranged)
		{
			DoRangedAbilityAction();
		}
		else if (abilityType == AbilityType.Melee)
		{
			DoMeleeAbilityAction();
		}
		else if (abilityType == AbilityType.AOE)
		{
			DoAOEAbilityAction();
		}
		else if (abilityType == AbilityType.ETC)
		{
			DoETCAbilityAction();
		}
	}

	private void DoRangedAbilityAction()
	{
		RangedAttack attack = (RangedAttack)TurnManager.Instance.CurrentTurn.m_Abilities[TurnManager.Instance.CurrentTurn.m_SelectedAbility];

		attack.ShowAbilityVisual();
	}

	private void DoMeleeAbilityAction()
	{
	}

	private void DoAOEAbilityAction()
	{
	}

	private void DoETCAbilityAction()
	{
	}

	void GeneratePossibleMovesOld(TurnBasedAI unit)
	{
		var path = ConstantPath.Construct(unit.transform.position, unit.movementPoints * 1000 + 1);

		path.traversalProvider = unit.traversalProvider;

		// Schedule the path for calculation
		AstarPath.StartPath(path);

		// Force the path request to complete immediately
		// This assumes the graph is small enough that
		// this will not cause any lag
		path.BlockUntilCalculated();

	}
}