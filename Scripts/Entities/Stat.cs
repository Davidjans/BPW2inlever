using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EVN.Stats
{
    [CreateAssetMenu(fileName = "Stat", menuName = "StackedBeans/Stats/Stat")]
    public class Stat : ScriptableObject
    {
        [SerializeField] private String m_ID = "stat-id";
        [SerializeField] private float m_BaseValue = 10;
        [ReadOnly]
        public float m_CurrentValue = 10;
        [SerializeField] private StatUIInfo m_UIInfo = new StatUIInfo();

        public Sprite Icon => m_UIInfo.m_Icon;
        public Color Color => m_UIInfo.m_Color;

        public float BaseValue => m_BaseValue;
        public String ID => m_ID;
        public String Acronym => m_UIInfo.m_Acronym;
        public String Name => m_UIInfo.m_Name;
        public String Description => m_UIInfo.m_Description;
    }
}

