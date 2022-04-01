using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class PlayerHandler : MonoBehaviour
    {
        public PlayerStats m_Player;

        [SerializeField] private Canvas m_Canvas;
        private bool m_SeeCanvas;

        private void Update()
        {
            if (Input.GetKeyDown("tab"))
            {
                if (m_Canvas)
                {
                    m_SeeCanvas = !m_SeeCanvas;
                    m_Canvas.gameObject.SetActive(m_SeeCanvas); //Enable or disable the canvas
                }
            }
        }
    }
}
