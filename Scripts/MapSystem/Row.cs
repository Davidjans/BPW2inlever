using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EVN.MapSystem
{
    public class Row : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_CostTextGUI;

        [ReadOnly] public int Index;
        [ReadOnly] public int Cost { get; private set; }

        public void SetCost(int cost)
        {
            Cost = cost;
            m_CostTextGUI.text = cost <= 0 ? "X" : cost.ToString();
        }

        [ReadOnly] public List<RoomNode> Nodes = new List<RoomNode>();
    }
}
