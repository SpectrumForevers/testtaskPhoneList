using UnityEngine;
using System.Net;
using System;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;

public class AppManager : MonoBehaviour
{
    [Header("URL for download data")]
    [SerializeField] string urlJson;
    [SerializeField] string urlImage;
    [Header(" ")]
    [SerializeField] string dataName;
    [Header(" ")]
    [SerializeField] int countDownloadForFirstStartApp = 5;

    private string resourceFolderName = "DownloadedImages";

    [SerializeField] List<Texture2D> resources = new List<Texture2D>();
    [SerializeField] GameObject TESTTEST;
    private void Awake()
    {
        if (PlayerPrefs.GetInt(Prefs.firstStartApp) == 0)
        {
            DownloadFile();
            for (int i = 0; i < countDownloadForFirstStartApp; i++)
            {
                DownloadImage();
            }
        }
        
    }
    private void Update()
    {
        if(resources.Count > countDownloadForFirstStartApp)
        {
            TESTTEST.GetComponent<Image>().sprite = Sprite.Create(resources[0], new Rect(0, 0, resources[0].width, resources[0].height), Vector2.zero);
        }
    }
    void DownloadFile()
    {
        WebClient webClient = new WebClient();

        //webClient.DownloadProgressChanged += DownloadProgressChanged;
        webClient.DownloadFileCompleted += DownloadComplete;

        webClient.DownloadFileAsync(new Uri(urlJson), Application.dataPath+$"/{dataName}.json");
    }
    private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        //Debug.Log("Progress = " + e.ProgressPercentage + " %");
    }
    private void DownloadComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        if (e.Error == null)
        {
            Debug.Log("Download compite");

        }
        else
        {
            Debug.Log($"Error: {e.Error}");
        }
    }
    public void DownloadImage()
    {
        StartCoroutine(DownloadImageCoroutine());
    }
    private IEnumerator DownloadImageCoroutine()
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(urlImage))
        {
            // �������� ���������� �������
            yield return www.SendWebRequest();

            // �������� �� ������� ������
            if (www.result == UnityWebRequest.Result.Success)
            {
                // ��������� �������� �� ����������� ������
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                
                // ��������� ����������� ����� �����
                string filename = GenerateUniqueFilename("downloaded_image", "png");
                
                // ��������� �������������� ���� � ����� Resources
                string relativePath = Path.Combine(resourceFolderName, filename);

                // ���������� ����������� � ����� Resources
                SaveTextureToResources(texture, relativePath);
                resources.Add(texture);
                
            }
            else
            {
                // ��������� ������ ��� �������� �����������
                Debug.LogError("������ �������� �����������: " + www.error);
            }
        }
    }

    // ����� ��� ��������� ����������� ����� �����
    private string GenerateUniqueFilename(string baseName, string extension)
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        string guid = System.Guid.NewGuid().ToString("N");
        return $"{baseName}_{timestamp}_{guid}.{extension}";
    }

    // ����� ��� ���������� �������� � ����� Resources
    private void SaveTextureToResources(Texture2D texture, string relativePath)
    {
        // �������� �����, ���� � ���
        string resourceFolderPath = Path.Combine(Application.dataPath, "Resources", resourceFolderName);
        Directory.CreateDirectory(resourceFolderPath);

        // ���������� �������� � ���� � ����� Resources
        byte[] bytes = texture.EncodeToPNG();
        string filePath = Path.Combine(Application.dataPath, "Resources", relativePath);
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("����������� ��������� � ����: " + relativePath);
    }
}
