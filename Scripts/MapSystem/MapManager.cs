using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EVN.MapSystem
{
    public class MapManager : MonoBehaviour
    {

        public static MapManager Instance;

        private RingMap m_ActiveMap;
        private List<RingMap> m_RingMaps = new List<RingMap>();
        private RoomAssets m_RoomAssets;

        [FoldoutGroup("Static Settings")]
        [SerializeField]
        private RingMap m_RingMapPrefab;

        [FoldoutGroup("Static Settings")]
        [SerializeField]
        private ScrollRect m_ScrollRect;

        [FoldoutGroup("Static Settings")]
        [SerializeField]
        private TextMeshProUGUI m_IntelText;

        [FoldoutGroup("Static Settings")]
        [SerializeField]
        private Transform m_ScrollRectViewport;

        [FoldoutGroup("Static Settings")] [SerializeField]
        private GameObject m_ContainingCanvas;
        
        [FoldoutGroup("Testing")]
        [SerializeField] private RingMapSettings m_TestRingMapSettings;
        [FoldoutGroup("Testing")]
        [SerializeField] private int m_StartingIntelAmmount = 15;

        public int Intel { get; private set; }


        private void Awake()
        {
            // Create a singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            m_RoomAssets = new RoomAssets();
        }


        private void Start()
        {
            SetIntel(m_StartingIntelAmmount);
            SetRingMapActive(AddRingMap(m_TestRingMapSettings));
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.P))
            {
                m_ActiveMap.MovePlayer(m_ActiveMap.m_Rows[m_ActiveMap.ActiveNode.Row.Index + 1].Nodes[0]);
            }
        }

        public void EnableMap()
        {
            gameObject.SetActive(true);
        }

        public void SetIntel(int intel)
        {
            Intel = intel;
            m_IntelText.text = intel.ToString();
        }

        public void SetRingMapActive(RingMap mapToSetActive)
        {
            // Show the ring map and hide the others
            for (int i = 0; i < m_RingMaps.Count; i++)
            {
                m_RingMaps[i].gameObject.SetActive(m_RingMaps[i] == mapToSetActive);
            }

            mapToSetActive.gameObject.SetActive(true);
            m_ScrollRect.content = (RectTransform)mapToSetActive.transform;

            m_ActiveMap = mapToSetActive;
        }

        public RingMap AddRingMap(RingMapSettings mapSettings)
        {     
            RingMap newMap = Instantiate(m_RingMapPrefab, m_ScrollRectViewport);
            newMap.GenerateMap(mapSettings, m_RoomAssets,this);

            m_RingMaps.Add(newMap);
            newMap.gameObject.SetActive(false);

            return newMap;
        }
        public void ActivateCanvas()
        {
            m_ContainingCanvas.SetActive(false);
        }

        public void DeActivateCanvas()
        {
            m_ContainingCanvas.SetActive(true);
        }
    }

    /// <summary>
    /// This class will find and so contain all the types of room/mapSystem related assets
    /// </summary>
    public class RoomAssets
    {
        private Dictionary<RoomType, GlobalRoomSettings> m_GlobalRoomSettings;
        private Dictionary<RoomType, List<RoomSettings>> m_Rooms;

        public RoomAssets()
        {
            // Crate new Dictionarys
            m_GlobalRoomSettings = new Dictionary<RoomType, GlobalRoomSettings>();
            m_Rooms = new Dictionary<RoomType, List<RoomSettings>>();

            // Load Global Rooms
            Object[] globalRooms = Resources.LoadAll("GlobalRooms", typeof(GlobalRoomSettings));

            for (int i = 0; i < globalRooms.Length; i++)
            {
                var settings = (GlobalRoomSettings)globalRooms[i];
                m_GlobalRoomSettings.Add(settings.RoomType, settings);
            }

            // Load all rooms
            Object[] rooms = Resources.LoadAll("Rooms", typeof(RoomSettings));

            for (int i = 0; i < rooms.Length; i++)
            {
                var room = (RoomSettings)rooms[i];

                // If the key does not exist add a new list
                if (!m_Rooms.ContainsKey(room.RoomType))
                {
                    m_Rooms.Add(room.RoomType, new List<RoomSettings>());
                }

                m_Rooms[room.RoomType].Add(room);
            }
        }


        public RoomSettings GetRandomRoom(RoomType roomType)
        {
            if(!m_Rooms.ContainsKey(roomType))
            {
                Debug.LogError("There is no room to pick from, create a new room!");
            }

            List<RoomSettings> roomSettings = m_Rooms[roomType];
            return roomSettings[Random.Range(0, roomSettings.Count)];
        }

        public List<RoomSettings> GetRoomsFromType(RoomType roomType)
        {
            if (!m_Rooms.ContainsKey(roomType))
            {
                Debug.LogError("There is no room to pick from, create a new room!");
            }

            return m_Rooms[roomType];
        }

        public GlobalRoomSettings GetGlobalRoom(RoomType roomType)
        {
            if (!m_GlobalRoomSettings.ContainsKey(roomType))
            {
                Debug.LogError("There is no global room to pick from, create a new room!");
            }

            return m_GlobalRoomSettings[roomType];
        }
        
    }
    
    
    

    // !!!Important!!!  NEVER REMOVE ITEM FROM THE LIST, ONLY ADD AT THE BOTTOM OR REPLACE
    public enum RoomType
    {
        FirstRoom,
        CombatEncounter,
        Shop,
        Boss,
        Fountain,
        Event,
    }

    // !!!Important!!!  NEVER REMOVE ITEM FROM THE LIST, ONLY ADD AT THE BOTTOM OR REPLACE
    public enum SideInfoType
    {
        Chest,
        HostileType,
        RoomHazards,
        RoomModifyers,
        Secret
    }

    public enum RingType
    {
        Ring1,
        Ring2,
        Ring3
    }
}