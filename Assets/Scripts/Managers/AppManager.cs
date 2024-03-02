using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

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

            //LoadImages();
            LoadFilesFromDirectory(PlayerPrefs.GetString(Prefs.filePathImage));
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
        
        string[] imagePaths = Directory.GetFiles(pathImage, "*.png");

        
        foreach (string path in imagePaths)
        {
            byte[] fileData = File.ReadAllBytes(path);

            
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            resources.Add(texture);
        }
    }

    void ReadJsonFile()
    {
        LoadFilesFromDirectory(PlayerPrefs.GetString(Prefs.filePathJson));
        if (File.Exists(pathJson))
        {
            
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
                SaveFile(jsonData, dataName);
                System.IO.File.WriteAllText(savePath, jsonData);
                Debug.Log("JSON saved to " + savePath);
            }
            else
            {
                Debug.LogError("Failed to download JSON: " + webRequest.error);
            }
        }
    }
    public void SaveFile(string data, string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(data);
                PlayerPrefs.SetString(Prefs.filePathJson, filePath);
                Debug.Log("String saved successfully: " + filePath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving file: " + e.Message);
        }
    }
    public void LoadFilesFromDirectory(string directoryPath)
    {
        try
        {
            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath);

                foreach (string filePath in files)
                {
                    string fileName = Path.GetFileName(filePath);
                    byte[] fileData = File.ReadAllBytes(filePath);

                    // Здесь вы можете обрабатывать данные, загруженные из каждого файла
                    // Например, можно определить тип файла по расширению и соответствующим образом его обработать
                    if (fileName.EndsWith(".png"))
                    {
                        Texture2D texture = new Texture2D(2, 2);
                        texture.LoadImage(fileData);
                        resources.Add(texture);
                    }
                    else if (fileName.EndsWith(".json"))
                    {
                        // Обработка файла JSON
                        jsonData = File.ReadAllText(filePath);
                        
                    }
                    else
                    {
                        // Другие типы файлов
                        Debug.Log("Loaded file: " + fileName);
                    }
                }
            }
            else
            {
                Debug.LogError("Directory not found: " + directoryPath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading files from directory: " + e.Message);
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
            
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                
                string filename = GenerateUniqueFilename("downloaded_image", "png");
                
                string relativePath = Path.Combine(resourceFolderName, filename);
                PlayerPrefs.SetString(Prefs.filePathImage, relativePath);
                SaveTextureToResources(texture, relativePath);
                resources.Add(texture);
            }
            else
            {
                Debug.LogError("Ошибка загрузки изображения: " + www.error);
            }
        }
    }

    
    private string GenerateUniqueFilename(string baseName, string extension)
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        string guid = System.Guid.NewGuid().ToString("N");
        return $"{baseName}_{timestamp}_{guid}.{extension}";
    }

    
    private void SaveTextureToResources(Texture2D texture, string relativePath)
    {

        string resourceFolderPath = Path.Combine(Application.dataPath, "Resources", resourceFolderName);
        Directory.CreateDirectory(resourceFolderPath);


        byte[] bytes = texture.EncodeToPNG();
        string filePath = Path.Combine(Application.dataPath, "Resources", relativePath);
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("Изображение сохранено в файл: " + relativePath);
    }
}
