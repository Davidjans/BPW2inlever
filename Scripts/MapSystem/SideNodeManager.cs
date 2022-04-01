using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EVN.MapSystem
{
    public class SideNodeManager : MonoBehaviour
    {
        public RoomNode m_ParentNode;
        public List<SideNode> m_SideNodesOwned = new List<SideNode>();
        public bool m_Expanded = false;
    
        private void Start()
        {
            if (m_ParentNode == null)
                m_ParentNode = GetComponentInParent<RoomNode>();
            m_SideNodesOwned = GetComponentsInChildren<SideNode>().ToList();
            for (int i = 0; i < m_ParentNode.RoomSettings.m_AdditionalRoomInfo.Count; i++)
            {
                m_SideNodesOwned[i].m_AdditionalInfoIcon.sprite =
                    m_ParentNode.RoomSettings.m_AdditionalRoomInfo[i].m_AdditionalInfoImage;
            }

            foreach (var sideNode in m_SideNodesOwned)
            {
                if (sideNode.m_AdditionalInfoIcon.sprite == null)
                {
                    Destroy(sideNode.gameObject);
                }
            }
        }
    }
}

