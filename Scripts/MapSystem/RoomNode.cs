using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EVN.MapSystem
{
    public class RoomNode : MonoBehaviour, IPointerClickHandler
    {
        [FoldoutGroup("ReadOnly")] [ReadOnly] public bool Revealed { get; private set; }
        [FoldoutGroup("ReadOnly")] [ReadOnly] public bool Available { get; private set; }
        [FoldoutGroup("ReadOnly")] [ReadOnly] public bool Active { get; private set; }

        [FoldoutGroup("ReadOnly")] [ReadOnly] public int Index;
        [FoldoutGroup("ReadOnly")] [ReadOnly] public Row Row;
        [FoldoutGroup("ReadOnly")] [ReadOnly] public RingMap ParentRingMap;
        [FoldoutGroup("ReadOnly")] [ReadOnly] public List<RoomNode> Exits = new List<RoomNode>();
        [FoldoutGroup("ReadOnly")] [ReadOnly] public List<RoomNode> Entrances = new List<RoomNode>();
        

        [FoldoutGroup("ReadOnly")]
        [ReadOnly]
        [SerializeField]
        public RoomSettings RoomSettings { get; private set; }

        [FoldoutGroup("ReadOnly")]
        [ReadOnly] 
        [SerializeField] 
        private GlobalRoomSettings m_GlobalRoomSettings;


        [FoldoutGroup("Node Settings")]
        [SerializeField] private Image m_NodeIcon;
        [SerializeField] private Animator m_NodeAnimator;

        [FoldoutGroup("Assigning")] public SideNodeManager m_SideNodeManager;

        private void Start()
        {
            m_NodeAnimator.Play("BecomeActive_Reverse", 1, 1);
            m_NodeAnimator.Play("RevealNode_Reverse", 2, 1);
            m_NodeAnimator.Play("BecomeAvailible_Reverse", 3, 1);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnPressLeft();
            }
            else if (eventData.button == PointerEventData.InputButton.Middle)
            {
                
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                OnPressRight();
            }
        }
        
        public void OnPressLeft()
        {
            if (!ParentRingMap.ActiveNode.Exits.Contains(this))
            {
                TryToRevealHidden();
            }
            else
            {
                TryToMoveTo();
            }
        }
    
        public void OnPressRight()
        {
            if (Revealed)
            {
                m_SideNodeManager.m_Expanded = !m_SideNodeManager.m_Expanded;
                m_NodeAnimator.SetBool("ExpandSide", m_SideNodeManager.m_Expanded);
            }
        }

        private void TryToRevealHidden()
        {
            bool unlocked = false;

            // If the node can be unlocked
            if (Row.Cost <= ParentRingMap.MapManager.Intel && Available)
            {
                ParentRingMap.MapManager.SetIntel(ParentRingMap.MapManager.Intel - Row.Cost);
               
                SetRevealed(true);

                ParentRingMap.UpdateAvalibleNodes();

                unlocked = true;
            }

            if (!unlocked)
            {
                m_NodeAnimator.SetTrigger("FailPress");
            }
        }

        private void TryToMoveTo()
        {
            ParentRingMap.MovePlayer(this);
        }

        public void SetAvailable(bool available)
        {
            Available = available;
        }

        public void SetRevealed(bool revealed)
        {
            Revealed = revealed;
        }

        public void SetActive(bool active)
        {
            Active = active;
        }


        // Animator needs to update every frame for no reason but it only works here so thats why maybe look at it in the future lol hello person reading this
        private void LateUpdate()
        {
            m_NodeAnimator.SetBool("Available", Available);
            m_NodeAnimator.SetBool("Revealed", Revealed);
            m_NodeAnimator.SetBool("Active", Active);
        }


        public void SetRoomSettings(RoomSettings roomSettings, GlobalRoomSettings globalRoomSettings)
        {
            RoomSettings = roomSettings;
            m_GlobalRoomSettings = globalRoomSettings;

            m_NodeIcon.sprite = m_GlobalRoomSettings.RoomIcon;
        }
    }
}