using UnityEngine;
using System.IO;

public class PlayerDataManager : MonoBehaviour
{
    private const string PlayerDataKey = "PlayerData";

    public static void SavePlayerData(PlayerData playerData)
    {
        // Convert the PlayerData object to a JSON string
        string jsonData = JsonUtility.ToJson(playerData);

        // Save the JSON string to PlayerPrefs
        PlayerPrefs.SetString(PlayerDataKey, jsonData);
        PlayerPrefs.Save();
    }

    public static PlayerData LoadPlayerData()
    {
        // Check if the PlayerData key exists in PlayerPrefs
        if (PlayerPrefs.HasKey(PlayerDataKey))
        {
            // Retrieve the JSON string from PlayerPrefs
            string jsonData = PlayerPrefs.GetString(PlayerDataKey);

            // Convert the JSON string back to a PlayerData object
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonData);
            return playerData;
        }

        // If no data exists, return null or a new PlayerData object
        return null;
    }

    public static Texture2D LoadPicture(string picturePath)
    {
        // Combine the picture path with the application's data path
        string fullPath = Path.Combine(Application.persistentDataPath, picturePath);

        if (File.Exists(fullPath))
        {
            byte[] fileData = File.ReadAllBytes(fullPath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            return texture;
        }
        else
        {
            Debug.LogError("Picture file not found at: " + fullPath);
            return null;
        }
    }
}