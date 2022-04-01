using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    [CreateAssetMenu(menuName = "Skill System/Player/Create Attribute")]
    public class Attributes : ScriptableObject
    {
        public string Description;
        public Sprite Thumbnail;
    }
}
