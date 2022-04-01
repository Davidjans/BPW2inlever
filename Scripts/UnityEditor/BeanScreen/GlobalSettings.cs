using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class GlobalSettings 
{
    //EditorWindow m_GlobalSettingsWindow;
    #region LodSettings
    [HideInInspector] public bool m_LODSetting;
    [HideIf("m_LODSetting")]
    [Button(ButtonSizes.Medium), GUIColor(1, 0.2f, 0)]
    private void LodsDisabled()
    {
        m_LODSetting = true;
        DoLod();
    }

    [ShowIf("m_LODSetting")]
    [Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
    private void LodsEnabled()
    {
        m_LODSetting = false;
        DoLod();
    }
    #endregion
    #region Shadow Settings
    [HideInInspector] public bool m_ShadowSetting;
    [HideIf("m_ShadowSetting")]
    [Button(ButtonSizes.Medium), GUIColor(1, 0.2f, 0)]
    private void ShadowsDisabled()
    {
        m_ShadowSetting = true;
        DoShadows();
    }

    [ShowIf("m_ShadowSetting")]
    [Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
    private void ShadowsEnabled()
    {
        m_ShadowSetting = false;
        DoShadows();
    }
    #endregion
    #region Bakery Settings
    [HideInInspector] public bool m_BakerySettings;
    [HideIf("m_BakerySettings")]
    [Button(ButtonSizes.Medium), GUIColor(1, 0.2f, 0)]
    private void BakeryLightsDisabled()
    {
        m_BakerySettings = true;
    }

    [ShowIf("m_BakerySettings")]
    [Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
    private void BakeryLightsEnabled()
    {
        m_BakerySettings = false;
    }
    #endregion
    #region LogSettings
    [HideInInspector] public bool m_EnableLogs;
    [HideIf("m_EnableLogs")]
    [Button(ButtonSizes.Medium), GUIColor(1, 0.2f, 0)]
    private void LogsDisabled()
    {
        m_EnableLogs = true;
        DoLogs();
    }

    [ShowIf("m_EnableLogs")]
    [Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
    private void LogsEnabled()
    {
        m_EnableLogs = false;
        DoLogs();
    }
    #endregion
    #region DevMode
    [HideInInspector] public bool m_EnableDevMode;
    [HideIf("m_EnableDevMode")]
    [Button(ButtonSizes.Medium), GUIColor(1, 0.2f, 0)]
    public void DevmodeDisabled()
    {
        m_EnableDevMode = true;
        DoDevMode();
    }

    [ShowIf("m_EnableDevMode")]
    [Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
    public void DevmodeEnabled()
    {
        m_EnableDevMode = false;
        DoDevMode();
    }
    #endregion
    #region StillNeedToDoDisplay
    [HideInInspector] public bool m_EnableStillDo = false;
    [HideIf("m_EnableStillDo")]
    [Button(ButtonSizes.Medium), GUIColor(1, 0.2f, 0)]
    private void NotDisplayingToDo()
    {
        m_EnableStillDo = true;
        DoToDo();
    }

    [ShowIf("m_EnableStillDo")]
    [Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
    private void DisplayingToDo()
    {
        m_EnableStillDo = false;
        DoToDo();
    }
    #endregion
    [HideInInspector] public LODGroup[] m_LodGroups;

    [HideInInspector] public MeshRenderer[] m_MeshRenderers;
    //[MenuItem("Wolfdog/PantherTools(vandavid)/GlobalSettings")]
    //static void Init()
    //{
    //    GlobalSettings GlobalSettingsWindow = (GlobalSettings)EditorWindow.GetWindow(typeof(GlobalSettings));
    //}
    public void Start()
    {
        m_LodGroups = Resources.FindObjectsOfTypeAll<LODGroup>();
        m_MeshRenderers = Resources.FindObjectsOfTypeAll<MeshRenderer>();
        // DoOnGui();
        GetValues();
    }

    void OnEnable()
    {
        //m_GlobalSettingsWindow = GetWindow(typeof(GlobalSettings));
        m_LodGroups = Resources.FindObjectsOfTypeAll<LODGroup>();
        m_MeshRenderers = Resources.FindObjectsOfTypeAll<MeshRenderer>();
        // DoOnGui();
        GetValues();
        Debug.Log("doing enable");
    }

    private void GetValues()
    {
        m_LODSetting = FileBasedPrefs.GetBool("LodSetting", true);
        m_BakerySettings = FileBasedPrefs.GetBool("BakerySettings", false);
        m_ShadowSetting = FileBasedPrefs.GetBool("ShadowSetting", true);
        m_EnableLogs = FileBasedPrefs.GetBool("EnableLogs", false);
        m_EnableDevMode = FileBasedPrefs.GetBool("EnableDevmode", false);
        m_EnableStillDo = FileBasedPrefs.GetBool("ToDoDisplay", false);
    }

    //private void OnGUI()
    //{
    //    DoOnGui();
    //}

    private void DoLod()
    {
        if (m_LODSetting != FileBasedPrefs.GetBool("LodSetting", true))
        {
            SetLods();
        }
        FileBasedPrefs.SetBool("LodSetting", m_LODSetting);
    }

    private void DoShadows()
    {
        if (m_ShadowSetting != FileBasedPrefs.GetBool("ShadowSetting", true))
        {
            SetShadows();
        }
        FileBasedPrefs.SetBool("ShadowSetting", m_ShadowSetting);
    }

    private void DoLogs()
    {
        FileBasedPrefs.SetBool("EnableLogs", m_EnableLogs);
    }

    private void DoToDo()
    {
        FileBasedPrefs.SetBool("ToDoDisplay", m_EnableStillDo);
    }

    private void DoDevMode()
    {
        FileBasedPrefs.SetBool("EnableDevmode", m_EnableDevMode);
    }

    //private void DoOnGui()
    //{
        //m_LODSetting = EditorGUILayout.Toggle("LodSetting", FileBasedPrefs.GetBool("LodSetting", true));
        //if (m_LODSetting != FileBasedPrefs.GetBool("LodSetting", true))
        //{
        //    SetLods();
        //}
        //FileBasedPrefs.SetBool("LodSetting", m_LODSetting);
        //Debug.Log(m_BakerySettings);
        //m_BakerySettings = EditorGUILayout.Toggle("BakerySettings", FileBasedPrefs.GetBool("BakerySettings", false));
        //if (m_BakerySettings != FileBasedPrefs.GetBool("BakerySettings", false))
        //{
        //    Debug.Log(m_BakerySettings + " comes here");
        //    SetBakeryLights();
        //}
        //FileBasedPrefs.SetBool("BakerySettings", m_BakerySettings);

        //m_ShadowSetting = EditorGUILayout.Toggle("ShadowSetting", FileBasedPrefs.GetBool("ShadowSetting", true));
        //if (m_ShadowSetting != FileBasedPrefs.GetBool("ShadowSetting", true))
        //{
        //    SetShadows();
        //}
        //FileBasedPrefs.SetBool("ShadowSetting", m_ShadowSetting);

        //m_EnableLogs = EditorGUILayout.Toggle("EnableLogs", FileBasedPrefs.GetBool("EnableLogs", false));
        //if (m_LogSetting != FileBasedPrefs.GetBool("LogSetting", true))
        //{
        //    SetLogs();
        //}
        //FileBasedPrefs.SetBool("EnableLogs", m_EnableLogs);



        //_EnableDevMode = EditorGUILayout.Toggle("EnableDevmode", FileBasedPrefs.GetBool("EnableDevmode", false));;

        //FileBasedPrefs.SetBool("EnableDevmode", m_EnableDevMode);
    //}
    private void ExecuteSettings()
    {
        SetLods();
    }

    private void SetLods()
    {
        m_LodGroups = Resources.FindObjectsOfTypeAll<LODGroup>();
        for (int i = 0; i < m_LodGroups.Length; i++)
        {
            m_LodGroups[i].enabled = m_LODSetting;
        }
    }

    private void SetShadows()
    {
        m_MeshRenderers = Resources.FindObjectsOfTypeAll<MeshRenderer>();
        for (int i = 0; i < m_MeshRenderers.Length; i++)
        {
            m_MeshRenderers[i].receiveShadows = m_ShadowSetting;
            if(m_ShadowSetting)
                m_MeshRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            else
                m_MeshRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
