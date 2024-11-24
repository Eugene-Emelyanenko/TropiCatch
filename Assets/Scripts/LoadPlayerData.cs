using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadPlayerData : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_InputField playerNotesInput;
    [SerializeField] private RawImage pictureImage;
    [SerializeField] private TextMeshProUGUI totalScoreText;

    public int maxSize = 200;

    private PlayerData playerData = null;

    private void Awake()
    {
        playerData = PlayerDataManager.LoadPlayerData();
    }

    private void Start()
    {
        if (playerData != null)
        {
            playerNameInput.text = playerData.playerName;
            playerNotesInput.text = playerData.notes;
            totalScoreText.text = playerData.totalScore.ToString();
            if (playerData.picture != "NULL")
            {
                pictureImage.texture =
                    ImageHelper.ResizeTexture(PlayerDataManager.LoadPicture(playerData.picture), maxSize);
                pictureImage.SetNativeSize();
            }
        }
        else
        {
            playerNameInput.text = "NULL";
            playerNotesInput.text = "NULL";
            Debug.LogError("Data is not founded");
        }
    }
    
    public void PickImage()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize: -1);
                if (texture != null)
                {
                    Texture2D resizedTexture = ImageHelper.ResizeTexture(texture, maxSize);
                    pictureImage.texture = resizedTexture;
                    pictureImage.SetNativeSize();
        
                    RenderTexture renderTexture = new RenderTexture(resizedTexture.width, resizedTexture.height, 0);
                    RenderTexture.active = renderTexture;
                    Material material = new Material(Shader.Find("UI/Default"));
                    material.mainTexture = resizedTexture;
                    Graphics.Blit(resizedTexture, renderTexture, material);
                    Texture2D newTexture = new Texture2D(resizedTexture.width, resizedTexture.height);
                    newTexture.ReadPixels(new Rect(0, 0, resizedTexture.width, resizedTexture.height), 0, 0);
                    newTexture.Apply();
                    string newPicture = ImageHelper.GenerateUniqueName();
                    string savePath = Path.Combine(Application.persistentDataPath, newPicture);
                    byte[] bytes = newTexture.EncodeToPNG();
                    File.WriteAllBytes(savePath, bytes);
                    Debug.Log("Изображение сохранено по пути: " + savePath);
                    RenderTexture.active = null;
                    Destroy(renderTexture);

                    playerData.picture = newPicture;
                    PlayerDataManager.SavePlayerData(playerData);
                }
            }
        }, "Select image", "image/*");
        Debug.Log("Permission result: " + permission);
    }

    public void ChangeName(string value)
    {
        if (value != string.Empty)
        {
            playerData.playerName = value;
            PlayerDataManager.SavePlayerData(playerData);
        }
    }
    
    public void ChangeNotes(string value)
    {
        if (value != string.Empty)
        {
            playerData.notes = value;
            PlayerDataManager.SavePlayerData(playerData);
        }
    }
}
