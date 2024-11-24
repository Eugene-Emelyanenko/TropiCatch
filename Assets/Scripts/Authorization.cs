using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Authorization : MonoBehaviour
{
    [SerializeField] private GameObject createProfileMenu;
    [SerializeField] private GameObject newNameMenu;
    [SerializeField] private GameObject newNotesMenu;
    [SerializeField] private GameObject newPictureMenu;
    [SerializeField] private GameObject instructionsMenu;
    
    [SerializeField] private TMP_InputField newNameInput;
    [SerializeField] private TMP_InputField newNotesInput;
    [SerializeField] private RawImage pictureImage;
    
    public int maxSize = 500;
    
    private string newPlayerName = string.Empty;
    private string newNotes = string.Empty;
    private string newPicture = string.Empty;

    private Texture2D pictureTexture2D = null;

    private void Start()
    {
        createProfileMenu.SetActive(false);
        newNameMenu.SetActive(true);
        newNotesMenu.SetActive(false);
        newPictureMenu.SetActive(false);
        instructionsMenu.SetActive(false);
    }

    public void Login()
    {
        PlayerData playerData = PlayerDataManager.LoadPlayerData();
        if (playerData != null)
        {
            Debug.Log($"Authorized. Name: {playerData.playerName}. Notes: {playerData.notes}. Picture: {playerData.picture}. Total score: {playerData.totalScore}");
            SceneManager.LoadScene("Main");
        }
        else
        {
            Debug.Log($"Player data not founded");
            createProfileMenu.SetActive(true);
        }
    }

    public void SetNewName()
    {
        string playerName = newNameInput.text.Trim();
        if (playerName != string.Empty)
        {
            newPlayerName = playerName;
            newNameMenu.SetActive(false);
            newPictureMenu.SetActive(true);
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
                    pictureTexture2D = ImageHelper.ResizeTexture(texture, maxSize);
                    pictureImage.texture = pictureTexture2D;
                    pictureImage.SetNativeSize();
                }
            }
        }, "Select image", "image/*");
        Debug.Log("Permission result: " + permission);
    }

    public void SetNewPicture()
    {
        if (pictureTexture2D == null)
        {
            Debug.LogError("Picture is null");
            return;
        }
        
        RenderTexture renderTexture = new RenderTexture(pictureTexture2D.width, pictureTexture2D.height, 0);
        RenderTexture.active = renderTexture;
        Material material = new Material(Shader.Find("UI/Default"));
        material.mainTexture = pictureTexture2D;
        Graphics.Blit(pictureTexture2D, renderTexture, material);
        Texture2D newTexture = new Texture2D(pictureTexture2D.width, pictureTexture2D.height);
        newTexture.ReadPixels(new Rect(0, 0, pictureTexture2D.width, pictureTexture2D.height), 0, 0);
        newTexture.Apply();
        newPicture = ImageHelper.GenerateUniqueName();
        string savePath = Path.Combine(Application.persistentDataPath, newPicture);
        byte[] bytes = newTexture.EncodeToPNG();
        File.WriteAllBytes(savePath, bytes);
        Debug.Log("Изображение сохранено по пути: " + savePath);
        RenderTexture.active = null;
        Destroy(renderTexture);
        
        newPictureMenu.SetActive(false);
        newNotesMenu.SetActive(true);
    }

    public void NotSetPicture()
    {
        newPicture = "NULL";
        newPictureMenu.SetActive(false);
        newNotesMenu.SetActive(true);
    }
    
    public void SetNewNotes()
    {
        string notes = newNotesInput.text.Trim();
        if (notes != string.Empty)
        {
            newNotes = notes;
            newNotesMenu.SetActive(false);
            instructionsMenu.SetActive(true);
        }
    }

    public void NotSetNotes()
    {
        newNotes = string.Empty;
        newNotesMenu.SetActive(false);
        instructionsMenu.SetActive(true);
    }

    public void CreateNewProfile()
    {
        PlayerData playerData = new PlayerData(newPlayerName, newNotes, newPicture);
        PlayerDataManager.SavePlayerData(playerData);
        Debug.Log($"Authorized. Name: {playerData.playerName}. Notes: {playerData.notes}. Picture: {playerData.picture}");
        SceneManager.LoadScene("Main");
    }
}