using System.Collections;
using System.Collections.Generic;
using EVN;
using EVN.Entities;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    #region Instancing
    public static EntityManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EntityManager>();
                if (_instance == null)
                {
                    _instance = new GameObject("TurnManager").AddComponent<EntityManager>();
                }
            }
            return _instance;
        }
    }
    private static EntityManager _instance;
    
    private void Awake()
    {
        // Destroy any duplicate instances that may have been created
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        _instance = this;
    }
    #endregion

    public List<MultipleTargetsCamera> m_MultiTargetCameras = new List<MultipleTargetsCamera>();
    public List<Entity> m_AliveEntities = new List<Entity>();

    public void AddEntity(Entity entity)
    {
        m_AliveEntities.Add(entity);
        foreach (var camera in m_MultiTargetCameras)
        {
            camera.m_Targets.Add(entity.transform);
        }
    }
    
    public void RemoveEntity(Entity entity)
    {
        m_AliveEntities.Remove(entity);
        foreach (var camera in m_MultiTargetCameras)
        {
            camera.m_Targets.Remove(entity.transform);
        }
    }

    public void AddMultiCamera(MultipleTargetsCamera camera)
    {
        m_MultiTargetCameras.Add(camera);
    }
    public void RemoveMultiCamera(MultipleTargetsCamera camera)
    {
        m_MultiTargetCameras.Remove(camera);
    }
}
