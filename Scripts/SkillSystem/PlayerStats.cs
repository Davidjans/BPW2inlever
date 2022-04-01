using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Main Player Stats")]
        public string m_PlayerName;
        public int m_PlayerHP = 50;

        [SerializeField] private int m_PlayerXP = 0;
        public int PlayerXP
        {
            get { return m_PlayerXP;  }
            set
            {
                m_PlayerXP = value;

                onXPChange?.Invoke();
            }
        }

        [SerializeField] private int m_PlayerLevel = 1;
        public int PlayerLevel
        {
            get { return m_PlayerLevel;  }
            set
            {
                m_PlayerLevel = value;

                onLevelChange?.Invoke();
            }
        }
        

        [Header("Player Attributes")]
        public List<PlayerAttributes> m_Attributes = new List<PlayerAttributes>();

        [Header("Player Skills Enabled")]
        public List<Skills> m_PlayerSkills = new List<Skills>();

        public delegate void OnXPChange();
        public event OnXPChange onXPChange;

        public delegate void OnLevelChange();
        public event OnLevelChange onLevelChange;

        public void UpdateLevel(int amount)
        {
            PlayerLevel += amount;
        }
        public void UpdateXP(int amount)
        {
            m_PlayerXP += amount;
        }
    }
}
