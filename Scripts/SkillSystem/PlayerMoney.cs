using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class PlayerMoney : MonoBehaviour
    {
        public static PlayerMoney m_PlayerMoney;

        public float m_GoldAmount;

        private void Awake()
        {
            m_PlayerMoney = this;
            m_GoldAmount = PlayerPrefs.GetFloat("GoldPieces");
        }

        public void AddMoney(float _amount)
        {
            m_GoldAmount += _amount;
            PlayerPrefs.SetFloat("GoldPieces", m_GoldAmount);
        }

        public void RemoveMoney(float _amount)
        {
            m_GoldAmount -= _amount;
            PlayerPrefs.SetFloat("GoldPieces", m_GoldAmount);
        }
    }
}
