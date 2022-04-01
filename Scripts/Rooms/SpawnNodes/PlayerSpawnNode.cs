using System.Collections;
using System.Collections.Generic;
using EVN.Movement;
using UnityEngine;
using EVN.Entities;
public class PlayerSpawnNode : EntitySpawnNode
{
    protected override void Start()
    {
        base.Start();
        PathfindingManager.Instance.m_CurrentMovingCharacter = m_SpawnedObject.GetComponent<Player>().m_Movement;
    }
}
