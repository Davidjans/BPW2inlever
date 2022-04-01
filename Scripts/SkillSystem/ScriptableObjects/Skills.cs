using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SkillSystem
{
    [CreateAssetMenu(menuName = "StackedBeans/Skill System/Player/Create Skill")]
    public class Skills : ScriptableObject
    {
        public string m_Description;
        public Sprite m_Icon;
        public int m_LevelNeeded;
        public int m_XPNeeded;
        public int m_GoldPiecesNeeded;
        public bool m_IsUnlocked;
        public bool m_IsUnlockable;
        public bool m_IsBought;
        //public bool m_IsSelectedTemporary;

        public List<Skills> m_NecessaryUnlockedSkills = new List<Skills>();
        public List<Ability> m_AbilitiesToUnlock = new List<Ability>();

        public void SetUnlockState()
        {
            if(!m_NecessaryUnlockedSkills.Any())
            {
                m_IsUnlockable = true;
            }
            else
            {
                for (int i = 0; i < m_NecessaryUnlockedSkills.Count; i++)
                {
                    if (m_NecessaryUnlockedSkills[i].m_IsUnlocked)
                    {
                        m_IsUnlockable = true;
                        break;
                    }
                    else
                    {
                        m_IsUnlockable = false;
                    }
                }
            }
        }

        public void SetValues(GameObject _skillDisplayObject, PlayerStats Player)
        {
            if (_skillDisplayObject)
            {
                SkillDisplay _skillDisplay = _skillDisplayObject.GetComponent<SkillDisplay>();
                _skillDisplay.m_SkillName.text = name;
                if (_skillDisplay.m_SkillDescription)
                {
                    _skillDisplay.m_SkillDescription.text = m_Description;
                }
                if (_skillDisplay.m_AbilityIcon)
                {
                    _skillDisplay.m_AbilityIcon.sprite = m_Icon;
                }
                if (_skillDisplay.m_SkillLevel)
                {
                    _skillDisplay.m_SkillLevel.text = m_LevelNeeded.ToString();
                }
                if (_skillDisplay.m_SkillGPNeeded)
                {
                    _skillDisplay.m_SkillGPNeeded.text = m_GoldPiecesNeeded.ToString() + "GP";
                }
                //if (_skillDisplay.m_SkillAttribute && m_AffectedAttributes.Any())
                //{
                //    _skillDisplay.m_SkillAttribute.text = m_AffectedAttributes[0].attributes.ToString();
                //}
                //if (_skillDisplay.m_SkillAttributeAmount && m_AffectedAttributes.Any())
                //{
                //    _skillDisplay.m_SkillAttributeAmount.text = "+" + m_AffectedAttributes[0].amount.ToString();
                //}
            }
        }

        public bool BuyAbility()
        {
            if(PlayerMoney.m_PlayerMoney.m_GoldAmount >= m_GoldPiecesNeeded)
            {
                PlayerMoney.m_PlayerMoney.RemoveMoney(m_GoldPiecesNeeded);
                for (int i = 0; i < m_AbilitiesToUnlock.Count; i++)
                {
                    m_AbilitiesToUnlock[i].m_IsAbilityBought = true;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckSkills(PlayerStats _player)
        {
            if(_player.PlayerLevel < m_LevelNeeded)
            {
                return false;
            }

            //Skills don't get bought with XP anymore
            //if (_player.PlayerXP < m_XPNeeded)
            //{
            //    return false;
            //}

            return true;
        }

        public bool EnableSkill(PlayerStats _player)
        {
            List<Skills>.Enumerator skills = _player.m_PlayerSkills.GetEnumerator();
            while(skills.MoveNext())
            {
                var currentSkill = skills.Current;
                if(currentSkill.name == name)
                {
                    return true;
                }
            }
            return false;
        }

       public void GetAbility()
       {
            for (int i = 0; i < m_AbilitiesToUnlock.Count; i++)
            {
                m_AbilitiesToUnlock[i].m_IsAbilityUnlocked = true;
            }
           //int i = 0;
           ////List<PlayerAttributes>.Enumerator attributes = m_AffectedAttributes.GetEnumerator();
           //List<PlayerAttributes>.Enumerator playerAttributes = _player.m_Attributes.GetEnumerator();
           //while (attributes.MoveNext())
           //{
           //    while(playerAttributes.MoveNext())
           //    {
           //        if(attributes.Current.attributes.name.ToString() == playerAttributes.Current.attributes.name.ToString())
           //        {
           //            playerAttributes.Current.amount += attributes.Current.amount;
           //            i++;
           //        }
           //    }
           //    if(i > 0)
           //    {
           //        _player.PlayerXP -= m_XPNeeded;
           //
           //        m_IsUnlocked = true;
           //        _player.m_PlayerSkills.Add(this);
           //        return true;
           //    }
           //}
           //return false;
       }

        public void LockAbilities()
        {
            for (int i = 0; i < m_AbilitiesToUnlock.Count; i++)
            {
                m_AbilitiesToUnlock[i].m_IsAbilityUnlocked = false;
            }
        }
    }
}
