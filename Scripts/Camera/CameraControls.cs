using UnityEngine;
using System.Collections;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem;
using UnityEngine.Experimental;
using UnityEngine.InputSystem.Utilities;

public class CameraControls : MonoBehaviour
{
    #region Instancing
    public static CameraControls Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CameraControls>();
                if (_instance == null)
                {
                    _instance = new GameObject("CameraControls").AddComponent<CameraControls>();
                }
            }
            return _instance;
        }
    }

    private static CameraControls _instance;
    #endregion
    public static CameraControls m_CameraControls;
    [Header("Movement")]
    [SerializeField] private float m_MovementSpeed = 20f;
    [SerializeField] private float m_BorderThickness = 10f;
    [SerializeField] private bool m_MouseMovementEnabled = false;
    private Vector2 m_Movement;

    [Header("Rotation")]
    [SerializeField] private Transform m_TransformToCircleAround;
    [SerializeField] private float m_RotationSpeed = 50f;
    [SerializeField] private float m_RotationDirection = 0;


    [Header("Panning")]
    [SerializeField] private Transform m_ObjectToPanTo;
    [SerializeField] private float m_PanningSpeed = 0.15f;

    [Header("Object Follow")]
    [SerializeField] private Transform m_TransformToFollow;
    private bool m_IsFollowing = false;

    [Space]
    public bool m_CanMove = true;
    [SerializeField] private InputActionAsset m_CameraMoveInput;
    [SerializeField] private InputAction m_CameraRotateInput;
    [SerializeField] private CameraControlsManager m_ControlsManager;

    [SerializeField] private Vector3 m_RotationStartPosition;

    private bool m_MovePressed;
    private bool m_RotationPressed;


    private void Awake()
    {
        m_ControlsManager = new CameraControlsManager();
        m_CameraControls = this;
        m_ControlsManager.CameraControls.Move.performed += context => m_MovePressed = true;
        m_ControlsManager.CameraControls.Move.canceled += context => m_MovePressed = false;

        m_ControlsManager.CameraControls.RightMouseHold.performed += context => m_RotationPressed = true;
        m_ControlsManager.CameraControls.RightMouseHold.canceled += context => m_RotationPressed = false;
        if (_instance == null)
        {
            _instance = this;
        }
        if (_instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }


    private void OnEnable()
    {
        m_ControlsManager.Enable();
    }
    private void Start()
    {
        m_RotationStartPosition = m_TransformToCircleAround.localPosition;
        
       LoadSettings();
    }

    void Update()
    {
        if (m_CanMove)
        {
            //MoveCam();
            if (!m_IsFollowing)
            {
                Movement();
                Rotation();
            }
            if (m_MovePressed)
            {
                MoveCam(m_ControlsManager.CameraControls.Move.ReadValue<Vector2>());
            }
            else
            {
                m_Movement = Vector2.zero;
            }
            if (m_RotationPressed)
            {
                RotateCam(m_ControlsManager.CameraControls.Rotate.ReadValue<Vector2>());
            }
            else
            {
                m_RotationDirection = 0;
            }
        }
        if (m_IsFollowing)
        {
            //transform.position = m_TransformToFollow.position;
            transform.position = new Vector3(m_TransformToFollow.position.x, m_TransformToFollow.position.y, m_TransformToFollow.position.z - 27);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            m_MouseMovementEnabled = !m_MouseMovementEnabled;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            FollowObject();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            StopFollowing();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Pan(0.25f));
        }

        if (Input.GetKey(KeyCode.V))
        {
            //var rebindOperation = m_ControlsManager.CameraControls.Move.PerformInteractiveRebinding()
            ////To avoid accidental input from mouse motion
            //.WithControlsExcluding("Mouse")
            //.OnMatchWaitForAnother(0.1f)
            //.Start();
            //StartCoroutine(Pan2(0.15f));
        }

        //Here is everything to make it work with keypresses, hopefully temporary
        if(m_CanMove)
        {
            //RotateWithKeys();
            //MoveWithKeys();
        }
    }

    private void RotateWithKeys()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            m_RotationDirection = -1;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            m_RotationDirection = 1;
        }
        else
        {
            m_RotationDirection = 0;
        }
    }

    private void MoveWithKeys()
    {
        if (Input.GetKey(KeyCode.W))
        {
            m_Movement.y = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            m_Movement.y = -1;
        }
        else
        {
            m_Movement.y = 0;
        }

        if (Input.GetKey(KeyCode.A))
        {
            m_Movement.x = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            m_Movement.x = 1;
        }
        else
        {
            m_Movement.x = 0;
        }
    }

    public void RotateCam(Vector2 ctx)
    {
        m_RotationDirection = Mathf.Clamp(ctx.x, -3, 3);
    }

    public void MoveCam(Vector2 ctx)
    {
        m_Movement = ctx;
    }

    private void Rotation()
    {
        if (m_RotationDirection != 0)
        {
            transform.RotateAround(m_TransformToCircleAround.position, -Vector3.up, (m_RotationDirection * m_RotationSpeed) * Time.deltaTime);
        }
    }

    public void FollowObject()
    {
        m_ObjectToPanTo = m_TransformToFollow;
        StartCoroutine(Pan(0.25f));
        StartCoroutine(LerpRotation(0.25f));
    }

    public void StopFollowing()
    {
        m_IsFollowing = false;
        m_ObjectToPanTo = null;
    }

    public void StartPan()
    {
        StartCoroutine(Pan(0.25f));
    }

    private IEnumerator LerpRotation(float duration)
    {
        float time = 0;
        Quaternion _startRotation = transform.rotation;
        Quaternion _enemyVec = Quaternion.identity;
        while (time < duration)
        {
            float _float = Mathf.SmoothStep(0, 1, time / duration);
            //float _float1 = Mathf.SmoothStep(0, 27, time / duration);
            transform.rotation = Quaternion.Lerp(_startRotation, _enemyVec, _float);
            //transform.position -= transform.forward * _float1;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        m_IsFollowing = true;
        //Vector3 _vec = /*transform.InverseTransformPoint(*/m_ObjectToPanTo.position;
        //transform.localPosition = new Vector3(_vec.x, _vec.y, _vec.z - 27);
    }

    private void Movement()
    {
        if (m_Movement != Vector2.zero)
        {
            transform.Translate(transform.forward * m_MovementSpeed * (m_Movement.y * Time.deltaTime), Space.World);
            transform.Translate(transform.right * m_MovementSpeed * (m_Movement.x * Time.deltaTime), Space.World);

            if (Input.mousePosition.y >= Screen.height - m_BorderThickness && m_MouseMovementEnabled)
            {
                transform.Translate(transform.forward * m_MovementSpeed * Time.deltaTime, Space.World);
            }
            if (Input.mousePosition.y <= m_BorderThickness && m_MouseMovementEnabled)
            {
                transform.Translate(-transform.forward * m_MovementSpeed * Time.deltaTime, Space.World);
            }
            if (Input.mousePosition.x <= m_BorderThickness && m_MouseMovementEnabled)
            {
                transform.Translate(-transform.right * m_MovementSpeed * Time.deltaTime, Space.World);
            }
            if (Input.mousePosition.x >= Screen.width - m_BorderThickness && m_MouseMovementEnabled)
            {
                transform.Translate(transform.right * m_MovementSpeed * Time.deltaTime, Space.World);
            }
        }
    }

    private IEnumerator Pan2(float duration)
    {
        //float _startTime = 0;
        //float t = 0;
        //while(transform.position != m_ActiveEnemy.position)
        //{
        //    //float t = time / duration;
        //    //t = t * t * (3f - 2f * t);
        //    //Debug.Log(t);
        //
        //    t = Time.time / duration;
        //    Debug.Log(Mathf.SmoothStep(0, 1, t));
        //    //transform.position = new Vector3(Mathf.SmoothStep(minimum, maximum, t), 0, 0);
        //    transform.position = Vector3.Lerp(transform.position, m_ActiveEnemy.position, Mathf.SmoothStep(0, 1, t));
        //}
        //yield return null;

        float time = 0;
        Vector3 startPosition = transform.localPosition;
        Vector3 _enemyVec = new Vector3(m_ObjectToPanTo.localPosition.x, m_ObjectToPanTo.localPosition.y, m_ObjectToPanTo.localPosition.z - 27f);

        while (time < duration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, _enemyVec, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = new Vector3(m_ObjectToPanTo.position.x, m_ObjectToPanTo.position.y, m_ObjectToPanTo.position.z - 27);
    }

    private IEnumerator Pan(float duration)
    {
        if (m_ObjectToPanTo)
        {
            float time = 0;
            Vector3 startPosition = transform.position;
            Vector3 _enemyVec = new Vector3(m_ObjectToPanTo.position.x, m_ObjectToPanTo.position.y, m_ObjectToPanTo.position.z);
            while (time < duration)
            {
                float _float = Mathf.SmoothStep(0, 1, time / duration);
                float _float1 = Mathf.SmoothStep(0, 27, time / duration);
                transform.position = Vector3.Lerp(startPosition, _enemyVec, _float);
                transform.position -= transform.forward * _float1;
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        //Vector3 _vec = /*transform.InverseTransformPoint(*/m_ObjectToPanTo.position;
        //transform.localPosition = new Vector3(_vec.x, _vec.y, _vec.z - 27);
    }

    public void ApplySettings()
    {
        Debug.Log("add rotation and movement saving");
        //m_RotationSpeed = PlayerPrefs.GetFloat("CameraRotationSpeed");
        //m_MovementSpeed = PlayerPrefs.GetFloat("CameraMovementSpeed");
    }

    public void LoadSettings()
    {
        Debug.Log("add rotation and movement loading");
        //m_RotationSpeed = PlayerPrefs.GetFloat("CameraRotationSpeed");
        //m_MovementSpeed = PlayerPrefs.GetFloat("CameraMovementSpeed");
    }
}
