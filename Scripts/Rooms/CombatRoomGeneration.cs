using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using EVN.RoomSystem;
using UnityEngine;

public class CombatRoomGeneration : RoomGeneration
{
    
    public GameObject m_EnemySpawnNode;

    protected async override void Start()
    {
        base.Start();
        DecideEnemySpawnPos();
        await Task.Delay(3000);
        CombatManager.Instance.StartCombat();
    }
    
    private void DecideEnemySpawnPos()
    {
        Vector3 enemySpawnPoint = Vector3.zero;
        enemySpawnPoint.z = m_RightTopROomPoint.z - (m_GridManager.m_CellSize /2);
        float xDiff = m_RightTopROomPoint.x - m_LeftBottomRoomPoint.x;
        enemySpawnPoint.x = Random.Range(xDiff * 0.3f, xDiff * 0.7f) + (m_GridManager.m_CellSize /2);
        enemySpawnPoint.y = m_LeftBottomRoomPoint.y;
        GameObject enemy = Instantiate(m_EnemySpawnNode);
        enemySpawnPoint.y += enemy.GetComponent<Collider>().bounds.extents.y;
        enemy.transform.position =  m_GridManager.m_Grid.GetCellOnPosition<MovementCell>(enemySpawnPoint).GetCellCenter();
    }
}
