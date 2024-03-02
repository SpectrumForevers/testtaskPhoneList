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
            //LoadFilesFromFolder(PlayerPrefs.GetString(Prefs.pathImage));
            //LoadFilesFromFolder(PlayerPrefs.GetString(Prefs.pathJson));
            
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
        
        if (File.Exists(pathJson))
        {
            
            jsonData = File.ReadAllText(pathJson);
        }
    }

    private void LoadFilesFromFolder(string folderPath)
    {
        folderPath = Path.GetDirectoryName(Path.GetFullPath(folderPath));

        if (Directory.Exists(folderPath))
        {
            string[] files = Directory.GetFiles(folderPath);

            foreach (string filePath in files)
            {
                string extension = Path.GetExtension(filePath);

                if (extension.Equals(".png", System.StringComparison.OrdinalIgnoreCase))
                {
                    byte[] fileData = File.ReadAllBytes(filePath);

                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(fileData);
                    resources.Add(texture);
                }
                else if (extension.Equals(".json", System.StringComparison.OrdinalIgnoreCase))
                {
                    
                    jsonData = File.ReadAllText(filePath);
                }
            }

            
            Debug.Log("Loaded " + resources.Count + " PNG files from folder: " + folderPath);

            
            Debug.Log("Loaded JSON data: " + jsonData);
        }
        else
        {
            Debug.LogError("Folder not found: " + folderPath);
        }
    
    }


private IEnumerator DownloadJson()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(urlJson);
        AsyncOperation asyncOperation = webRequest.SendWebRequest();

        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            jsonData = webRequest.downloadHandler.text;

            
            string fullPath = Path.Combine(Application.persistentDataPath, savePath);

            File.WriteAllText(fullPath, jsonData);
            PlayerPrefs.SetString(Prefs.pathJson, fullPath);
            pathJson = fullPath;
            
        }
        else
        {
            Debug.LogError("Failed to download JSON: " + webRequest.error);
        }
    }

public void DownloadImage()
    {
        StartCoroutine(DownloadImageCoroutine());
    }
    
    private IEnumerator DownloadImageCoroutine()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(urlImage);
        AsyncOperation asyncOperation = www.SendWebRequest();

        while (!asyncOperation.isDone)
        {
            yield return null; 
        }

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            string filename = GenerateUniqueFilename("downloaded_image", "png");
            string relativePath = Path.Combine(resourceFolderName, filename);

            
            string fullPath = Path.Combine(Application.persistentDataPath, relativePath);
            PlayerPrefs.SetString(Prefs.pathImage, fullPath);
            pathImage = fullPath;
            SaveTextureToPath(texture, fullPath);
            resources.Add(texture);
        }
        else
        {
            Debug.LogError("Ошибка загрузки изображения: " + www.error);
        }
    }

    private void SaveTextureToPath(Texture2D texture, string path)
    {
        if (Directory.Exists(Path.GetDirectoryName(Path.GetFullPath(path))))
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));

        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        Debug.Log("Image saved to " + path);
    }

    private string GenerateUniqueFilename(string baseName, string extension)
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        string guid = System.Guid.NewGuid().ToString("N");
        return $"{baseName}_{timestamp}_{guid}.{extension}";
    }

    
}
