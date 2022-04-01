using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVN.Stats
{
    [CreateAssetMenu(fileName = "Attribute", menuName = "StackedBeans/Stats/Attribute")]
    public class Attribute : Stat
    {
        public Stat m_StatToPerformCalcWith;
        public bool m_RoundResult = false;
        public float m_StatCalcImpact = 10;
        public float m_CurrentMaxValue = 10;
    }
}
