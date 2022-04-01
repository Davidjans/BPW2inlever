using System.Collections;
using System.Collections.Generic;
using EVN.Entities;
using TMPro;
using UnityEngine;

public class TargetInfo : MonoBehaviour
{
    public Entity m_CastingEntity;
    public Entity m_TargetEntity;
    public TextMeshProUGUI m_NameText;
    public TextMeshProUGUI m_ACText;
    public TextMeshProUGUI m_HPText;
    public TextMeshProUGUI m_HitChanceText;

    public void SetTargets(Entity castingEntity, Entity targetEntity)
    {
        gameObject.SetActive(true);
        m_CastingEntity = castingEntity;
        m_TargetEntity = targetEntity;
        SetTargetInfo();
    }
    
    public void SetTargetInfo()
    {
        m_NameText.text = "Name: " + m_TargetEntity.name;
        m_ACText.text = "AC: " + m_TargetEntity.m_AttributeDictionary["AC"].m_CurrentValue;
        m_HPText.text = "HP: " + m_TargetEntity.m_AttributeDictionary["Health"].m_CurrentValue;
        if (m_CastingEntity != null)
        {
            float neededRoll = m_TargetEntity.m_AttributeDictionary["AC"].m_CurrentValue -
                               m_CastingEntity.GetToHitBonus();
            float percentageChance = Mathf.Clamp(1 - (neededRoll / 20), 0,1) ;
            percentageChance *= 100;
            m_HitChanceText.text = "Hit%: " + percentageChance+"%";
        }
        else
        {
            m_HitChanceText.text = "HitBonus: " + m_TargetEntity.GetToHitBonus();
        }
    }
}
