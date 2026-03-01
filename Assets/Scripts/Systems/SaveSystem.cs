using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static GridSystem;

public class SaveSystem
{
    private static string SavePath => Application.persistentDataPath + "/save.json";

    public static void SaveGameData(GameSaveData data) {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Game Saved: " + SavePath);
    }

    public static GameSaveData GetSavedGameData() {
        if (!File.Exists(SavePath)) {
            Debug.LogWarning("No save file found.");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public static string GetSaveFilePath() {
        return SavePath;
    }

    public static void DeleteSave() {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
    }

    public static bool HasSave() {
        return File.Exists(SavePath);
    }

    public static GameSaveData CreateSavableData(SlotData[,] slots, int gridX, int gridY, int score, int turns, float time) {
        GameSaveData data = new GameSaveData();

        data.columns = gridX;
        data.rows = gridY;

        data.hasCard = new List<bool>();
        data.cardTypes = new List<int>();
        data.matchedStates = new List<bool>();

        for (int x = 0; x < gridX; x++) { // Loop through each column in the grid
            for (int y = 0; y < gridY; y++) { // Loop through each slot in the grid

                SlotData slot = slots[x, y];

                if (slot.card != null) {
                    data.hasCard.Add(true);
                    data.cardTypes.Add((int)slot.card.GetCardType());
                    data.matchedStates.Add(slot.card.IsMatched());
                }
                else {
                    data.hasCard.Add(false);
                    data.cardTypes.Add(0);      // dummy
                    data.matchedStates.Add(false);
                }
            }
        }

        data.score = score;
        data.turns = turns;
        data.elapsedTime = time;

        return data;
    }
}


[System.Serializable]
public class GameSaveData {
    public int columns;
    public int rows;

    public List<bool> hasCard; // Add hasCard list to track which slots have cards and which are empty
    public List<int> cardTypes;      // Add card types to track which cards are in which positions
    public List<bool> matchedStates;  // Add matched states to track which cards have been matched and should remain flipped
   
    public int score;
    public int turns;
    public float elapsedTime;
}