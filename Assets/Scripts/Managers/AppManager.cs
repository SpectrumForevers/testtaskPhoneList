using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class AppManager : MonoBehaviour
{
    [Header("URL for download data")]
    [SerializeField] string urlJson;
    [SerializeField] string urlImage;
    [Header(" ")]
    [SerializeField] string dataName;
    [Header(" ")]
    [SerializeField] int countDownloadForFirstStartApp = 5;
    [Header("Path for image")]
    [SerializeField] string pathImage;
    [Header("Path for json")]
    [SerializeField] string pathJson;
    private string resourceFolderName = "DownloadedImages";
    public string savePath = "Assets/Json/myJsonFile.json";
    [SerializeField] string jsonData;

    [SerializeField] List<Texture2D> resources = new List<Texture2D>();
    [SerializeField] bool clearAllPrefs = false;
    private void Awake()
    {
        if (PlayerPrefs.GetInt(Prefs.firstStartApp) == 0)
        {
            StartCoroutine(DownloadJson());
            for (int i = 0; i < countDownloadForFirstStartApp; i++)
            {
                DownloadImage();
            }
            PlayerPrefs.SetInt(Prefs.firstStartApp, 2);
        }
        else
        {
            LoadImages();
            ReadJsonFile();
        }
        if (clearAllPrefs == true)
        {
            PlayerPrefs.DeleteAll();
        }
    }
    private void Update()
    {
        if(resources.Count == countDownloadForFirstStartApp)
        {
            EventBus.listImages?.Invoke(resources);
        }
        if(jsonData != null)
        {
            EventBus.json?.Invoke(jsonData);
        }
    }
    void LoadImages()
    {
        // �������� ��� ����� �� ��������� �����
        string[] imagePaths = Directory.GetFiles(pathImage, "*.png"); // ������ �������� ���������� �����, ���� � ��� ������ �������

        // �������� �� ������� ����� � ��������� ��� � ��������
        foreach (string path in imagePaths)
        {
            byte[] fileData = File.ReadAllBytes(path);

            // ��������� ������ ����������� � �������� � ��������� �� � ������
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            resources.Add(texture);
        }

        // ������ � ��� ���� ������ �������, ������� �� ������ ������������ � ����� ����
    }

    void ReadJsonFile()
    {
        // ���������, ���������� �� ����
        if (File.Exists(pathJson))
        {
            // ������ ���� ����� �� �����
            jsonData = File.ReadAllText(pathJson);
        }
    }

            IEnumerator DownloadJson()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(urlJson))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                jsonData = webRequest.downloadHandler.text;

                System.IO.File.WriteAllText(savePath, jsonData);
                Debug.Log("JSON saved to " + savePath);
            }
            else
            {
                Debug.LogError("Failed to download JSON: " + webRequest.error);
            }
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
