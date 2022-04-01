using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[CreateAssetMenu(fileName = "Gear", menuName = "StackedBeans/Gear/BaseWeaponGear", order = 1)]
public class BaseWeaponGear : BaseGear
{
	public float m_BaseDamage;
	public string m_StatToScaleDamageWith;
	public float m_AmmountToScaleDamagePerStatPoint;
	public float m_BonusToHit;
	public string m_StatToScaleToHitWith;
	public float m_AmmountToScaleToHitPerStatPoint;
}
