using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Dit script is puur om shit te resetten in edit mode, gaat niet gebruikt worden in-game
namespace SkillSystem
{
    public class ResetSkills : MonoBehaviour
    {
        [SerializeField] private List<Skills> m_SkillsList = new List<Skills>();

        public void ResetUnlocked()
        {
            for (int i = 0; i < m_SkillsList.Count; i++)
            {
                m_SkillsList[i].m_IsUnlocked = false;
            }
        }

        public void ResetBought()
        {
            for (int i = 0; i < m_SkillsList.Count; i++)
            {
                for (int j = 0; j < m_SkillsList[i].m_AbilitiesToUnlock.Count; j++)
                {
                    m_SkillsList[i].m_AbilitiesToUnlock[j].m_IsAbilityBought = false;
                }
            }
        }
    }
}
