using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseAbility : MonoBehaviour
{
    [SerializeField] private Ability m_Ability;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && m_Ability.m_IsAbilityUnlocked)
        {
            m_Ability.AbilityAction();
        }
    }
}
