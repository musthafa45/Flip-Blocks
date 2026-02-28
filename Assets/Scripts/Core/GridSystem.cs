using System;
using UnityEngine;

public class GridSystem : MonoBehaviour {
    [Header("Grid Settings")]
    [Tooltip("Grid count in X (columns) and Y (rows)")]
    [SerializeField] private Vector2Int gridXY = new Vector2Int(2, 2);

    [SerializeField] private RectTransform gridHolderRectTransform;

    [SerializeField] private float spaceBetweenSlotsX = 10f;
    [SerializeField] private float spaceBetweenSlotsY = 0f;

    private SlotData[,] slots;

    private void Start() {
        GenerateGrid();
    }

    [ContextMenu("Generate Grid")]
    private void GenerateGrid() {
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

        for (int x = 0; x < columns; x++) {
            for (int y = 0; y < rows; y++) {

                float posX = (-width / 2f)
                            + (cellWidth * x)
                            + (spaceBetweenSlotsX * x)
                            + cellWidth / 2f;

                float posY = (-height / 2f)
                            + (cellHeight * y)
                            + (spaceBetweenSlotsY * y)
                            + cellHeight / 2f;

                Vector2 localPosition = new Vector2(posX, posY);

                GameObject point = new GameObject($"Slot_{x}_{y}");
                point.transform.SetParent(gridHolderRectTransform, false);

                RectTransform rect = point.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(10, 10);
                rect.anchoredPosition = localPosition;

                slots[x, y] = new SlotData {
                    slotTransform = rect
                };
            }
        }
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



    [ContextMenu("Reset Grid Item Positions")]
    public void ResetGridItemPositions() {
        if (slots == null) return;

        int columns = slots.GetLength(0);
        int rows = slots.GetLength(1);

        float width = gridHolderRectTransform.rect.width;
        float height = gridHolderRectTransform.rect.height;

        float totalSpacingX = (columns - 1) * spaceBetweenSlotsX;
        float totalSpacingY = (rows - 1) * spaceBetweenSlotsY;

        float usableWidth = width - totalSpacingX;
        float usableHeight = height - totalSpacingY;

        float cellWidth = usableWidth / columns;
        float cellHeight = usableHeight / rows;

        for (int x = 0; x < columns; x++) {
            for (int y = 0; y < rows; y++) {
                if (slots[x, y] == null || slots[x, y].slotTransform == null)
                    continue;

                float posX = (-width / 2f)
                             + (cellWidth * x)
                             + (spaceBetweenSlotsX * x)
                             + cellWidth / 2f;

                float posY = (-height / 2f)
                            + (cellHeight * y)
                            + (spaceBetweenSlotsY * y)
                            + cellHeight / 2f;

                RectTransform rect = slots[x, y].slotTransform as RectTransform;
                rect.anchoredPosition = new Vector2(posX, posY);
            }
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
    }
}