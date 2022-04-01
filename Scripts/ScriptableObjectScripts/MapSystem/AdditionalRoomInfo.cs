using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[CreateAssetMenu(fileName = "New Room Settings", menuName = "StackedBeans/MapSystem/AdditionalRoomInfo", order = 1)]
public class AdditionalRoomInfo : ScriptableObject
{
    public Sprite m_AdditionalInfoImage;
    public string m_AdditionalInfoText;
}
