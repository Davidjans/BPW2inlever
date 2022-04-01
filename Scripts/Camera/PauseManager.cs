using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject m_PauseCanvas;
    private CameraControls m_CameraControls;

    private bool m_IsPaused = true;

    private void Start()
    {
        m_CameraControls = CameraControls.m_CameraControls;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            m_IsPaused = !m_IsPaused;
            if(m_IsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        m_PauseCanvas.SetActive(true);
        m_CameraControls.m_CanMove = false;
    }

    public void Resume()
    {
        m_PauseCanvas.SetActive(false);
        m_CameraControls.m_CanMove = true;
    }
}
