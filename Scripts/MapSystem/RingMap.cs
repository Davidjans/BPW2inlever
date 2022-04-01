using Sirenix.OdinInspector;
using System.Collections.Generic;
using EVN.RoomSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace EVN.MapSystem
{
    public class RingMap : MonoBehaviour
    {

        [SerializeField] private GameObject m_RoomNodePrefab;
        [SerializeField] private GameObject m_RowPrefab;
        [SerializeField] private GameObject m_RowBackgroundPrefab;
        [SerializeField] private GameObject m_LinePrefab;
        [SerializeField] private RectTransform m_MapContentParentRect;
        [SerializeField] private RectTransform m_RowsParentRect;
        [SerializeField] private RectTransform m_RowBackgroundsParentRect;
        [SerializeField] private RectTransform m_LinesParentRect;
        [SerializeField] private HorizontalLayoutGroup m_rowsLayoutGroup;
        [SerializeField] private Color m_RowColor1;
        [SerializeField] private Color m_RowColor2;


        [Space(10f)]

        [ReadOnly] public Row[] m_Rows;
        [ReadOnly] public Line[] m_Lines;
        [ReadOnly] public RoomNode ActiveNode { get; private set; }
        [ReadOnly] public MapManager MapManager { get; private set; }
        private bool m_ShouldDoOnSceneLoaded = false;
        #region OnSceneLoaded
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // called second
        void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
        {
            if (m_ShouldDoOnSceneLoaded)
            {
                MapManager.gameObject.SetActive(false);
                RoomSceneManager roomSceneManager = FindObjectOfType<RoomSceneManager>();
                roomSceneManager.m_RoomsByRoomType[ActiveNode.RoomSettings.RoomType].m_RoomSettings = ActiveNode.RoomSettings;
                roomSceneManager.EnableAndGenerateRoom(ActiveNode.RoomSettings.RoomType);
                m_ShouldDoOnSceneLoaded = false;
            }
        }
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        #endregion

        public void MovePlayer(RoomNode toNode)
        {
            // Disable the old node
            if (ActiveNode != null)
                ActiveNode.SetActive(false);

            ActiveNode = toNode;

            UpdateRowCosts(ActiveNode.Row.Index);

            // Set the Active node to reveled
            ActiveNode.SetRevealed(true);
            ActiveNode.SetActive(true);
            
            m_ShouldDoOnSceneLoaded = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            
            foreach (var exit in ActiveNode.Exits)
            {
                exit.SetRevealed(true);
            }
            UpdateAvalibleNodes();
        }


        public void UpdateAvalibleNodes()
        {
            // Reset All Nodes
            for (int r = 0; r < m_Rows.Length; r++)
            {
                for (int n = 0; n < m_Rows[r].Nodes.Count; n++)
                {
                    
                    m_Rows[r].Nodes[n].SetAvailable(false);
                    // keep availible if already revealed
                    if(m_Rows[r].Nodes[n].Revealed)
                        m_Rows[r].Nodes[n].SetAvailable(true);
                }
            }


            // Set the first exits of the active node to availible
            for (int n = 0; n < ActiveNode.Exits.Count; n++)
            {
                // If there is not enough intel for the first exits return the function
                if (ActiveNode.Exits[n].Row.Cost > MapManager.Intel)
                    return;

                ActiveNode.Exits[n].SetAvailable(true);
            }


            // Set nodes available
            for (int r = 0; r < m_Rows.Length; r++)
            {
                if (m_Rows[r].Cost > MapManager.Intel)
                    break;

                for (int n = 0; n < m_Rows[r].Nodes.Count; n++)
                {
                    RoomNode checkNode = m_Rows[r].Nodes[n];

                    // Set the node to available if on of the entrances is also active
                    for (int e = 0; e < checkNode.Entrances.Count; e++)
                    {
                        if (checkNode.Entrances[e].Available)
                        {
                            checkNode.SetAvailable(true);
                            break;
                        }
                    }
                }
            }
        }

        #region MapGeneration

        /// <summary>
        /// Generates the map based on settings
        /// </summary>
        public void GenerateMap(RingMapSettings settings, RoomAssets roomAssets, MapManager mapManager)
        {

            // Set map manager
            MapManager = mapManager;

            // Create the rows
            SetContentRectSize(settings.Spacing, settings.RowCount);
            CreateRows(settings.RowCount);

            // Create the nodes
            CreateNodes(settings.RowCount, settings.MinRowHeight, settings.MaxRowHeight);
            PositionNodes(settings.SmoothnesScale, settings.MaxRowHeight, settings.Spacing);
            ConnectNodes(settings.MaxNodeConnectDistance);

            // Update Row Layout Before Creating lines
            UpdateRowsLayoutGroup();

            // Create the lines
            CreateLines();
            // Position Lines with a frame delay
            PositionLines();


            // Sets the rooms for every room node
            SetRoomOnNodes(settings.FirstRoomType, settings.lastRoomType, roomAssets, settings.ForcedRooms);

            // Move player to the first node
            MovePlayer(m_Rows[0].Nodes[0]);
        }


        /// <summary>
        /// Updates the cost for every row
        /// </summary>
        public void UpdateRowCosts(int playerPosition)
        {
            for (int r = 0; r < m_Rows.Length; r++)
            {
                int rowCost;

                if (playerPosition >= r)
                {
                    rowCost = 0;
                }
                else
                {
                    rowCost = r - playerPosition + 2;
                }

                m_Rows[r].SetCost(rowCost);
            }
        }



        /// <summary>
        /// Sets the size of the content rect
        /// </summary>
        private void SetContentRectSize(Vector2 spacing, int rowCount)
        {
            Vector2 m_contentRectSize = Vector2.zero;

            // Set the size x to the (spacing + the node with) * row count
            m_contentRectSize.x = (spacing.x + m_RoomNodePrefab.GetComponent<RectTransform>().rect.width) * rowCount;

            m_MapContentParentRect.sizeDelta = m_contentRectSize;
        }

        /// <summary>
        /// Creates/Spawns all the rows
        /// </summary>
        private void CreateRows(int rowCount)
        {
            // Fill the array
            m_Rows = new Row[rowCount];

            // Loop the array
            for (int i = 0; i < m_Rows.Length; i++)
            {
                // Get the components
                Row row = Instantiate(m_RowPrefab, m_RowsParentRect).GetComponent<Row>();
                m_Rows[i] = row;
                m_Rows[i].Index = i;

                // Spawn background for each row
                Image rowBackground = Instantiate(m_RowBackgroundPrefab, m_RowBackgroundsParentRect).GetComponent<Image>();
                rowBackground.color = i % 2 == 0 ? m_RowColor1 : m_RowColor2;
            }

        }



        /// <summary>
        /// Creates/Spawns all the nodes
        /// </summary>
        private void CreateNodes(int rowCount, int minRowHeight, int maxRowHeight)
        {
            // Loop trough all the room node rows
            for (int r = 0; r < rowCount; r++)
            {

                // Define how many nodes there are in this row
                int nodesInRow;

                // Check if the node is at the first or last row \\
                if (r == 0 || r == rowCount - 1)
                {
                    nodesInRow = 1;
                }
                else
                {
                    // Generate a random row height
                    nodesInRow = Random.Range(minRowHeight, maxRowHeight + 1);


                    // Make sure there is never 2 notes of 1 after each other \\

                    // If the current node is 1
                    if (nodesInRow == 1)
                    {
                        // If the left node if this is the second to last node set the node count to 2
                        if (m_Rows[r - 1].Nodes.Count == 1 || r == rowCount - 2)
                        {
                            // If the left or the right node is 1 then make this row 2
                            nodesInRow = 2;
                        }
                    }
                }


                // Spawn the Room Nodes At The Correct Position\\
                for (int n = 0; n < nodesInRow; n++)
                {
                    // Spawn the node \\
                    RoomNode roomNode = Instantiate(m_RoomNodePrefab, Vector3.zero, Quaternion.identity, m_Rows[r].transform).GetComponent<RoomNode>();

                    m_Rows[r].Nodes.Add(roomNode);

                    // Set the roomNode property's \\
                    roomNode.Row = m_Rows[r];
                    roomNode.Index = n;
                    roomNode.ParentRingMap = this;
                }
            }
        }

        /// <summary>
        /// Positions the nodes in the UI;
        /// </summary>
        private void PositionNodes(float smoothnesScale, int maxRowHeight, Vector2 spacing)
        {
            // Position node \\ 
            Vector2 anchoredPosition = Vector2.zero;

            for (int r = 0; r < m_Rows.Length; r++)
            {
                // Get the smoothing value for this row
                float smoothing = ((maxRowHeight / (float)m_Rows[r].Nodes.Count) - 1) * smoothnesScale;

                for (int n = 0; n < m_Rows[r].Nodes.Count; n++)
                {
                    RectTransform roomNodeRect = m_Rows[r].Nodes[n].GetComponent<RectTransform>();

                    // Calculate the spacing
                    float s = roomNodeRect.rect.height + spacing.y + smoothing;

                    anchoredPosition.y = (n * s) - ((m_Rows[r].Nodes.Count - 1) * s / 2.0f); ;

                    // Apply the position
                    roomNodeRect.anchoredPosition = anchoredPosition;
                }
            }

        }

        /// <summary>
        /// Connects all the node exits
        /// </summary>
        private void ConnectNodes(float maxNodeConnectDistance)
        {
            // Connect entrances \\
            for (int r = 1; r < m_Rows.Length; r++)
            {
                // Loop every colom in this row
                for (int n = 0; n < m_Rows[r].Nodes.Count; n++)
                {
                    RoomNode currentNode = m_Rows[r].Nodes[n];

                    RoomNode availibleNode = GetAvailibleNodeFromRow(m_Rows[r - 1], currentNode, maxNodeConnectDistance);
                    SetNodeExit(availibleNode, currentNode);
                }
            }


            // Clean up and Connect all exits \\
            for (int r = 1; r < m_Rows.Length - 1; r++)
            {
                // Loop every colom in this row
                for (int n = 0; n < m_Rows[r].Nodes.Count; n++)
                {
                    RoomNode currentNode = m_Rows[r].Nodes[n];

                    if (currentNode.Exits.Count == 0)
                    {
                        RoomNode availibleNode = GetAvailibleNodeFromRow(m_Rows[r + 1], currentNode, maxNodeConnectDistance);
                        SetNodeExit(currentNode, availibleNode);
                    }
                }
            }
        }



        /// <summary>
        /// Creates/Spawns all the lines 
        /// </summary>
        private void CreateLines()
        {
            List<Line> newLines = new List<Line>();

            for (int r = 0; r < m_Rows.Length; r++)
            {
                for (int n = 0; n < m_Rows[r].Nodes.Count; n++)
                {
                    for (int e = 0; e < m_Rows[r].Nodes[n].Exits.Count; e++)
                    {
                        RoomNode fromNode = m_Rows[r].Nodes[n];
                        RoomNode toNode = m_Rows[r].Nodes[n].Exits[e];

                        Line line = Instantiate(m_LinePrefab, m_LinesParentRect).GetComponent<Line>();

                        line.FromNode = fromNode;
                        line.ToNode = toNode;

                        newLines.Add(line);
                    }
                }
            }

            m_Lines = newLines.ToArray();
        }

        /// <summary>
        /// Updates the rows layout group;
        /// </summary>
        private void UpdateRowsLayoutGroup()
        {
            m_rowsLayoutGroup.CalculateLayoutInputHorizontal();
            m_rowsLayoutGroup.CalculateLayoutInputVertical();
            m_rowsLayoutGroup.SetLayoutHorizontal();
            m_rowsLayoutGroup.SetLayoutVertical();
        }

        /// <summary>
        /// Positions all the lines from to nodes
        /// </summary>
        private void PositionLines()
        {
            for (int i = 0; i < m_Lines.Length; i++)
            {
                m_Lines[i].PositionLine();
            }
        }



        private void SetRoomOnNodes(RoomType firstRoomType, RoomType lastRoomType, RoomAssets roomAssets, RingMapSettings.ForcedRoom[] roomsToBeForced)
        {

            // Get the forced rooms and the rows they take in \\

            Dictionary<int, RoomType> ForcedRooms = new Dictionary<int, RoomType>();


            for (int f = 0; f < roomsToBeForced.Length; f++)
            {
                RingMapSettings.ForcedRoom forcedRoom = roomsToBeForced[f];


                // Get Row Options \\

                List<int> rowOptions = new List<int>();

                // Get a list op row options
                int minRowOption = Mathf.Clamp(forcedRoom.PossibleAfterRow, 1, m_Rows.Length - 1);
                int maxRowOption = m_Rows.Length - 1;
                for (int r = minRowOption; r < maxRowOption; r++)
                {                 
                    // Skip row if its in use
                    if (ForcedRooms.ContainsKey(r))
                    {           
                        continue;
                    }

                    rowOptions.Add(r);
                }


                // Make sure the row options is not empty
                if (rowOptions.Count == 0)
                {
#if UNITY_EDITOR
                    Debug.LogError("There are no row options, try making sure the forced rooms are possible in the room with the row number");
#endif
                    return;
                }


                // Get a random Room from the optional rows for this forced room
                int randomRow = rowOptions[Random.Range(0, rowOptions.Count - 1)];

                ForcedRooms.Add(randomRow, forcedRoom.RoomType);
            }




            // Set all the rooms for every node \\

            // Loop all rows
            for (int r = 0; r < m_Rows.Length; r++)
            {
                // Set first and last room
                if (r == 0)
                {

                    m_Rows[r].Nodes[0].SetRoomSettings(roomAssets.GetRandomRoom(firstRoomType), roomAssets.GetGlobalRoom(firstRoomType));
                    continue;
                }

                if (r == m_Rows.Length - 1)
                {
                    m_Rows[r].Nodes[0].SetRoomSettings(roomAssets.GetRandomRoom(lastRoomType), roomAssets.GetGlobalRoom(lastRoomType));
                    continue;
                }


                // Loop every node in this row
                for (int n = 0; n < m_Rows[r].Nodes.Count; n++)
                {
                    // Get the node
                    RoomNode node = m_Rows[r].Nodes[n];


                    // Create the final room type and set the default to combat encounter;
                    RoomType finalRoomType = RoomType.CombatEncounter;

                    if (ForcedRooms.ContainsKey(r)) // If the row is already assigned to a forced row
                    {
                        finalRoomType = ForcedRooms[r];
                    }
                    else // If not the row will be randomly generated based on the previous nodes
                    {
                        // Add all options as possible
                        List<RoomType> roomOptions = new List<RoomType>();
                        foreach (RoomType roomType in System.Enum.GetValues(typeof(RoomType)))
                        {
                            roomOptions.Add(roomType);
                        }

                        // Loop all entrances
                        for (int i = 0; i < node.Entrances.Count; i++)
                        {
                            // Get the global room of entrance
                            GlobalRoomSettings globalRoom = roomAssets.GetGlobalRoom(node.Entrances[i].RoomSettings.RoomType);
                            List<EnumToggles.EnumToggle> enumToggles = globalRoom.ExitRoomOptions.Toggles;

                            for (int e = 0; e < enumToggles.Count; e++)
                            {
                                if (!enumToggles[e].m_enumEnabled)
                                {
                                    if (roomOptions.Contains((RoomType)e))
                                    {
                                        // If the room is not possible add it
                                        roomOptions.Remove((RoomType)e);
                                    }
                                }
                            }
                        }

                        // Get a random final room from the options
                        if (roomOptions.Count > 0)
                        {
                            finalRoomType = roomOptions[Random.Range(0, roomOptions.Count)];
                        }

                    }

                    node.SetRoomSettings(roomAssets.GetRandomRoom(finalRoomType),roomAssets.GetGlobalRoom(finalRoomType));

                    node.SetActive(false);
                    node.SetAvailable(false);
                    node.SetRevealed(false);
                }
            }

        }



        /// <summary>
        /// Sets the exit of a given node
        /// </summary>
        private void SetNodeExit(RoomNode baseNode, RoomNode exit)
        {
            // Add the exit
            baseNode.Exits.Add(exit);
            exit.Entrances.Add(baseNode);
        }

        /// <summary>
        /// Returns an available node from a row
        /// </summary>
        private RoomNode GetAvailibleNodeFromRow(Row rowToPickFrom, RoomNode fromNode, float maxNodeConnectDistance)
        {

            List<int> connectOptions = new List<int>();
            List<int> extraOptions = new List<int>();

            // Loop all the possible options
            for (int n = 0; n < rowToPickFrom.Nodes.Count; n++)
            {

                // Check distance
                if (CheckDistance(ref extraOptions, rowToPickFrom.Nodes[n], fromNode, maxNodeConnectDistance))
                {
                    continue;
                }

                // Check cross
                if (CheckCross(rowToPickFrom.Nodes[n], fromNode))
                {
                    continue;
                }

                // Add connection to options \\
                connectOptions.Add(n);
            }


            // In the case of no options replace the list with the extra options
            if (connectOptions.Count == 0)
            {
                connectOptions = extraOptions;
            }

            // pick a random option from the options
            return rowToPickFrom.Nodes[connectOptions[Random.Range(0, connectOptions.Count)]];
        }

        /// <summary>
        /// Returns true if the distance is to far and adds to the extra options so that there is always an exit
        /// </summary>
        private bool CheckDistance(ref List<int> extraOptions, RoomNode toNode, RoomNode fromNode, float maxNodeConnectDistance)
        {

            float previousNodeHeight = toNode.Index - (toNode.Row.Nodes.Count - 1) / 2f;
            float currentNodeHeight = fromNode.Index - (fromNode.Row.Nodes.Count - 1) / 2f;
            float nodeDistance = previousNodeHeight - currentNodeHeight;

            // If the node is to far away return
            if (nodeDistance > maxNodeConnectDistance)
            {
                extraOptions.Add(0);
                return true;
            }
            else if (nodeDistance < -maxNodeConnectDistance)
            {
                extraOptions.Add(toNode.Row.Nodes.Count - 1);
                return true;
            }

            return false;
        }

        /// <summary>
        ///  Checks if the nodes cross other lines
        /// </summary>
        private bool CheckCross(RoomNode node1, RoomNode node2)
        {

            RoomNode leftNode;
            RoomNode rightNode;

            // Set row sides
            if (node1.Row.Index > node2.Row.Index)
            {
                leftNode = node2;
                rightNode = node1;
            }
            else
            {
                leftNode = node1;
                rightNode = node2;
            }



            if (leftNode.Row.Nodes.Count == 1 || rightNode.Row.Nodes.Count == 1)
                return false;


            bool cross = false;


            // Option 1
            for (int l = leftNode.Index + 1; l < leftNode.Row.Nodes.Count; l++)
            {

                for (int r = rightNode.Index - 1; r >= 0; r--)
                {

                    if (leftNode.Row.Nodes[l].Exits.Contains(rightNode.Row.Nodes[r]))
                    {
                        cross = true;
                    }
                }
            }


            // Option 2
            for (int l = leftNode.Index - 1; l >= 0; l--)
            {
                for (int r = rightNode.Index + 1; r < rightNode.Row.Nodes.Count; r++)
                {
                    if (leftNode.Row.Nodes[l].Exits.Contains(rightNode.Row.Nodes[r]))
                    {
                        cross = true;
                    }
                }
            }

            return cross;
        }

        #endregion

    }

}