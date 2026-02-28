using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour {
    [Header("Grid Settings")]
    [Tooltip("Grid count in X (columns) and Y (rows)")]
    [SerializeField] private Vector2Int gridXY = new Vector2Int(2, 2);

    [SerializeField] private RectTransform gridHolderRectTransform;
    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private float spaceBetweenSlotsX = 10f;
    [SerializeField] private float spaceBetweenSlotsY = 0f;

    private SlotData[,] slots;
    private List<CardType> cardPool = new List<CardType>();


    public void InitializeGrid(Vector2Int gridSize) {
        gridXY = gridSize;
        GenerateGrid();
    }

    public int GetTotalSlots() {
        return gridXY.x * gridXY.y;
    }

    [ContextMenu("Delete Grid")]
    public void DeleteGrid() {
        if (slots == null) return;

        int columns = slots.GetLength(0);
        int rows = slots.GetLength(1);

        for (int x = 0; x < columns; x++) {
            for (int y = 0; y < rows; y++) {
                if (slots[x, y] != null && slots[x, y].slotTransform != null) {
                    Destroy(slots[x, y].slotTransform.gameObject);
                }
            }
        }
        slots = null;
    }

    //private void Start() {
    //    GenerateGrid();
    //}

    [ContextMenu("Generate Grid")]
    private void GenerateGrid() {
        DeleteGrid();

        int columns = gridXY.x;
        int rows = gridXY.y;

        slots = new SlotData[columns, rows];

        float width = gridHolderRectTransform.rect.width;
        float height = gridHolderRectTransform.rect.height;

        float totalSpacingX = (columns - 1) * spaceBetweenSlotsX;
        float totalSpacingY = (rows - 1) * spaceBetweenSlotsY;

        float usableWidth = width - totalSpacingX;
        float usableHeight = height - totalSpacingY;

        float cellWidth = usableWidth / columns;
        float cellHeight = usableHeight / rows;

        int totalSlots = columns * rows;
        int middleIndex = totalSlots / 2; // for odd grids

        bool isOdd = totalSlots % 2 != 0;
        int slotsToFill = isOdd ? totalSlots - 1 : totalSlots;

        // Generate card pool
        cardPool = GenerateCardPool(slotsToFill);
        int poolIndex = 0;

        for (int x = 0; x < columns; x++) {
            for (int y = 0; y < rows; y++) {
                int currentIndex = x * rows + y;

                // Skip middle slot if odd
                if (isOdd && currentIndex == middleIndex) {

                    GameObject emptySlot = new GameObject($"EmptySlot_{x}_{y}");
                    emptySlot.transform.SetParent(gridHolderRectTransform, false);

                    RectTransform emptyRect = emptySlot.AddComponent<RectTransform>();
                    emptyRect.sizeDelta = new Vector2(cellWidth, cellHeight);
                    emptyRect.anchoredPosition = new Vector2(
                        (-width / 2f) + (cellWidth * x) + (spaceBetweenSlotsX * x) + cellWidth / 2f,
                        (-height / 2f) + (cellHeight * y) + (spaceBetweenSlotsY * y) + cellHeight / 2f
                    );

                    slots[x, y] = new SlotData { slotTransform = emptyRect };
                    continue;
                }

                // Normal slot + card
                float posX = (-width / 2f) + (cellWidth * x) + (spaceBetweenSlotsX * x) + cellWidth / 2f;
                float posY = (-height / 2f) + (cellHeight * y) + (spaceBetweenSlotsY * y) + cellHeight / 2f;

                Vector2 localPosition = new Vector2(posX, posY);

                GameObject point = new GameObject($"Slot_{x}_{y}");
                point.transform.SetParent(gridHolderRectTransform, false);

                RectTransform rect = point.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(cellWidth, cellHeight);
                rect.anchoredPosition = localPosition;

                // Instantiate card
                GameObject cardInstance = Instantiate(cardPrefab, rect);
                cardInstance.transform.localPosition = Vector3.zero;

                CardType type = cardPool[poolIndex];
                Card card = cardInstance.GetComponent<Card>();
                card.InitializeCard(type);
                poolIndex++;

                slots[x, y] = new SlotData {
                    slotTransform = rect,
                    card = card
                };
            }
        }
    }

    private List<CardType> GenerateCardPool(int totalSlots) {
        if (totalSlots % 2 != 0) {
            Debug.LogError("Grid must have even number of slots for pairs!");
            return null;
        }

        List<CardType> pool = new List<CardType>();

        int pairCount = totalSlots / 2;

        for (int i = 0; i < pairCount; i++) {
            CardType type = (CardType)(i + 1); // assuming enum starts at 1

            pool.Add(type);
            pool.Add(type); // add pair
        }

        Shuffle(pool);

        return pool;
    }

    private void Shuffle(List<CardType> list) {
        for (int i = list.Count - 1; i > 0; i--) {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }


    private void OnDrawGizmos() {
        if (gridHolderRectTransform == null) return;
        if(slots == null) return;

        Gizmos.color = Color.green;

        int columns = gridXY.x;
        int rows = gridXY.y;

        for (int x = 0; x < columns; x++) {
            for (int y = 0; y < rows; y++) {
                if (slots[x, y] != null && slots[x, y].slotTransform != null) {
                    Gizmos.DrawSphere(slots[x,y].slotTransform.position,50f);
                }
                
            }
        }
    }

 

    [System.Serializable]
    public class SlotData {
        public Transform slotTransform;
        public Card card;
    }
}