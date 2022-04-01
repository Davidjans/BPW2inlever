using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnumToggles
{
    // Label name
    private string name; 

    [ListDrawerSettings(IsReadOnly = true, Expanded = true)]
    [LabelText("$name")]
    public List<EnumToggle> Toggles = new List<EnumToggle>();


    [HorizontalGroup("Group 1")]
    [Button(ButtonSizes.Medium)]
    private void EnableAll()
    {
        for (int i = 0; i < Toggles.Count; i++)
        {
            Toggles[i].m_enumEnabled = true;
        }
    }

    [HorizontalGroup("Group 1")]
    [Button(ButtonSizes.Medium)]
    private void DisableAll()
    {
        for (int i = 0; i < Toggles.Count; i++)
        {
            Toggles[i].m_enumEnabled = false;
        }
    }


    /// <summary>
    /// Returns an int list of all the enabled enums, cast to get the enum value
    /// </summary>
    public List<int> GetEnums()
    {
        List<int> enabledEnums = new List<int>();
        for (int i = 0; i < Toggles.Count; i++)
        {
            if (Toggles[i].m_enumEnabled)
            {
                enabledEnums.Add(i);
            }
        }

        return enabledEnums;
    }


    /// <summary>
    /// Call to update the inspector for the enum toggle
    /// </summary>
    public void ValidateEnumToggle(Type enumType, string inspectorName = "")
    {
        // Set the name
        name = inspectorName == "" ? enumType.Name : inspectorName;

        // Make sure the list is not null
        if(Toggles == null)
        {
            Toggles = new List<EnumToggle>();
        }

        // Get all the room enum names and store them in a list
        List<string> rooms = new List<string>();
        foreach (string name in Enum.GetNames(enumType))
        {
            rooms.Add(name);
        }


        // Add all the rooms to the list
        if (rooms.Count > Toggles.Count)
        {
            int times = rooms.Count - Toggles.Count;
            for (int i = 0; i < times; i++)
            {
                EnumToggle newToggle = new EnumToggle();
                newToggle.m_enumName = rooms[Toggles.Count].ToString();
                newToggle.m_enumEnabled = true;

                Toggles.Add(newToggle);
            }
        }
        else if (rooms.Count < Toggles.Count)
        {
            Toggles.RemoveAt(Toggles.Count - 1);
        }
    }


    [Serializable]
    public class EnumToggle
    {
        [HideInInspector]
        public string m_enumName;

        [LabelText("$m_enumName")]
        [LabelWidth(150f)]
        public bool m_enumEnabled;
    }

}
