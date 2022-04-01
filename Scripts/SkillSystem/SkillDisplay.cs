using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SkillSystem
{
    public class SkillDisplay : MonoBehaviour
    {
        public Skills m_Skill;
        [Header("UI Components")]
        public TextMeshProUGUI m_SkillName;
        public TextMeshProUGUI m_SkillDescription;
        public Image m_AbilityIcon;
        public TextMeshProUGUI m_SkillLevel;
        public TextMeshProUGUI m_SkillGPNeeded;
        public TextMeshProUGUI m_SkillAttribute;
        public TextMeshProUGUI m_SkillAttributeAmount;
        public Image m_BoughtIcon;
        public Button m_Button;

        [Space]
        [SerializeField] private PlayerStats m_PlayerHandler;
        [SerializeField] private bool m_InShop = false;
        [SerializeField] private bool m_InBuilderShop = false;

        private void Start()
        {
            if (!m_PlayerHandler)
            {
                m_PlayerHandler = GetComponentInParent<PlayerHandler>().m_Player;
            }

            m_PlayerHandler.onXPChange += ReactToChange;

            m_PlayerHandler.onLevelChange += ReactToChange;

            if (m_Skill)
            {
                m_Skill.SetValues(gameObject, m_PlayerHandler);
                m_Skill.SetUnlockState();
                //if (m_Skill.m_IsUnlocked)
                //{
                //    m_Button.interactable = true;
                //}
                //else
                //{
                //    m_Button.interactable = false;
                //}
            }

            EnableSkills();
        }

        public void EnableSkills()
        {
            if (m_InShop)
            {
                if (!CheckIfBought())
                {
                    CheckIfAffordable();
                }
            }
            else if (m_InBuilderShop)
            {
                if (m_Skill.m_IsUnlocked || !m_Skill.m_IsUnlockable)
                {
                    TurnOffSkillIcon();
                }
                else if (m_Skill.m_IsUnlockable)
                {
                    TurnOnSkillIcon();
                }
            }
            else
            {
                if (AreAbilitiesBought() && !AreAbilitiesUnlocked())
                {
                    if (m_PlayerHandler && m_Skill && /*m_Skill.EnableSkill(m_PlayerHandler) &&*/ m_Skill.CheckSkills(m_PlayerHandler))
                    {
                        TurnOnSkillIcon();
                    }
                    else
                    {
                        TurnOffSkillIcon();
                    }
                }
                else
                {
                    TurnOffSkillIcon();
                }
            }
        }

        private bool AreAbilitiesBought()
        {
            int index = 0;
            for (int i = 0; i < m_Skill.m_AbilitiesToUnlock.Count; i++)
            {
                if (m_Skill.m_AbilitiesToUnlock[i].m_IsAbilityBought)
                {
                    index++;
                }
            }

            if(index >= m_Skill.m_AbilitiesToUnlock.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool AreAbilitiesUnlocked()
        {
            int index = 0;
            for (int i = 0; i < m_Skill.m_AbilitiesToUnlock.Count; i++)
            {
                if (m_Skill.m_AbilitiesToUnlock[i].m_IsAbilityUnlocked)
                {
                    index++;
                }
            }

            if (index >= m_Skill.m_AbilitiesToUnlock.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetUnlockState()
        {
            m_Skill.SetUnlockState();
        }

        public void BuySkillFromShop()
        {
            //Checks if you can actually afford the skill
            if (m_Skill.BuyAbility())
            {
                m_SkillGPNeeded.gameObject.SetActive(false);
                m_Button.interactable = false;
                m_BoughtIcon.gameObject.SetActive(true);
            }
        }

        private bool CheckIfBought()
        {
            int index = 0;
            for (int i = 0; i < m_Skill.m_AbilitiesToUnlock.Count; i++)
            {
                if (m_Skill.m_AbilitiesToUnlock[i].m_IsAbilityBought)
                {
                    index++;
                }
            }

            if (index >= m_Skill.m_AbilitiesToUnlock.Count)
            {
                TurnOffSkillIcon();
                m_BoughtIcon.gameObject.SetActive(true);
                return true;
            }
            else
            {
                TurnOnSkillIcon();
                m_BoughtIcon.gameObject.SetActive(false);
                return false;
            }
        }

        //Sets the color of the price text according to if the player can or cannot afford it
        private void CheckIfAffordable()
        {
            if (PlayerMoney.m_PlayerMoney.m_GoldAmount < m_Skill.m_GoldPiecesNeeded)
            {
                m_SkillGPNeeded.color = Color.red;
                TurnOffSkillIcon();
            }
            else
            {
                m_SkillGPNeeded.color = Color.green;
                TurnOnSkillIcon();
            }
        }

        private bool CheckIfCanBuy()
        {
            if (m_Skill.m_NecessaryUnlockedSkills.Any())
            {
                List<bool> _bools = new List<bool>();
                for (int i = 0; i < m_Skill.m_NecessaryUnlockedSkills.Count; i++)
                {
                    _bools.Add(m_Skill.m_NecessaryUnlockedSkills[i].m_IsUnlocked);
                }
                if (!m_Skill.m_IsUnlocked && _bools.Contains(true))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private void OnEnable()
        {
            EnableSkills();
        }

        public void GetSkill()
        {
            m_Skill.GetAbility();
            TurnOnSkillIcon();
            //if (m_Skill.GetSKill(m_PlayerHandler))
            //{
            //    TurnOnSkillIcon();
            //}
        }

        public void UnlockSkill()
        {
            m_Skill.m_IsUnlocked = true;
        }

        public void RemoveRow()
        {
            m_PlayerHandler.onLevelChange -= ReactToChange;
            m_PlayerHandler.onXPChange -= ReactToChange;
        }

        private void TurnOffSkillIcon()
        {
            m_Button.interactable = false;
        }

        private void TurnOnSkillIcon()
        {
            m_Button.interactable = true;
        }

        private void OnDisable()
        {
            m_PlayerHandler.onXPChange -= ReactToChange;
        }

        private void ReactToChange()
        {
            EnableSkills();
        }
    }
}
