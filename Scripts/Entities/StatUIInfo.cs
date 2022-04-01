using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVN.Stats
{
    [Serializable]
    public class StatUIInfo
    {
        public String m_Name;
        public String m_Acronym;
        public String m_Description;
        public Sprite m_Icon;
        public Color m_Color;

        public StatUIInfo()
        {
            this.m_Icon = null;
            this.m_Color = Color.grey;
        }
    }
}