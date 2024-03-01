using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ImageDownloader : MonoBehaviour
{
    // Ссылки на изображения
    private string[] imageUrls = { "https://picsum.photos/200", "https://picsum.photos/201", "https://picsum.photos/202" };

    // Список для хранения скачанных текстур
    [SerializeField] private List<Texture2D> downloadedTextures = new List<Texture2D>();

    // Метод для загрузки, сохранения и изменения типа текстуры
    private void Awake()
    {
        DownloadSaveAndSetSpriteType();
        
    }
    private void Update()
    {
        if(downloadedTextures.Count > 2)
        {
            ConvertTexturesToSprites();
        }
    }
    public void DownloadSaveAndSetSpriteType()
    {
        StartCoroutine(DownloadImagesCoroutine());
    }

    // Корутина для асинхронной загрузки изображений
    private IEnumerator DownloadImagesCoroutine()
    {
        foreach (string imageUrl in imageUrls)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl))
            {
                // Ожидание завершения запроса
                yield return www.SendWebRequest();

                // Проверка на наличие ошибок
                if (www.result == UnityWebRequest.Result.Success)
                {
                    // Получение текстуры из загруженных данных
                    Texture2D texture = DownloadHandlerTexture.GetContent(www);

                    // Генерация уникального имени файла
                    string filename = GenerateUniqueFilename("downloaded_image", "png");

                    // Получение пути к папке Resources
                    string resourcesPath = "Assets/Resources/DownloadedImages";

                    // Сохранение изображения в папку Resources
                    SaveTextureToFile(texture, Path.Combine(resourcesPath, filename));

                    // Получение относительного пути к текстуре в папке Resources
                    string relativePath = Path.Combine("Assets/Resources/DownloadedImages", filename);

                    // Изменение типа текстуры на Sprite (2D and UI)
                    SetTextureImporterSettings(relativePath);

                    // Добавление текстуры в список
                    downloadedTextures.Add(texture);
                }
                else
                {
                    // Обработка ошибок при загрузке изображения
                    Debug.LogError("Ошибка загрузки изображения: " + www.error);
                }
            }
        }

        // Вызов метода для преобразования текстур в Sprite (2D and UI), после того как лист полностью заполнен
        //ConvertTexturesToSprites();
    }

    // Метод для генерации уникального имени файла
    private string GenerateUniqueFilename(string baseName, string extension)
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        string guid = System.Guid.NewGuid().ToString("N");
        return $"{baseName}_{timestamp}_{guid}.{extension}";
    }

    // Метод для сохранения текстуры в файл
    private void SaveTextureToFile(Texture2D texture, string filePath)
    {
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("Изображение сохранено в файл: " + filePath);
    }

    // Метод для изменения типа текстуры на Sprite (2D and UI)
    private void SetTextureImporterSettings(string relativePath)
    {
        // Определение пути к текстуре в редакторе ресурсов Unity
        string assetPath = relativePath.Substring(0, relativePath.LastIndexOf('.'));

        // Загрузка текстуры
        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

        // Проверка наличия текстуры и изменение настроек
        if (textureImporter != null)
        {
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spritePixelsPerUnit = 100; // Задайте нужное значение
            textureImporter.filterMode = FilterMode.Bilinear;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed; // Или другой метод сжатия, в зависимости от вашего выбора
            textureImporter.SaveAndReimport();
            Debug.Log("Тип текстуры изменен на Sprite (2D and UI): " + assetPath);
        }
        else
        {
            Debug.LogError("Ошибка при изменении типа текстуры. Текстура не найдена: " + assetPath);
        }
    }

    // Метод для преобразования текстур в Sprite (2D and UI)
    private void ConvertTexturesToSprites()
    {
        foreach (Texture2D texture in downloadedTextures)
        {
            // Ваши дополнительные действия с текстурой
            // ...

            // Пример: преобразование в Sprite (2D and UI)
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            // Ваши дополнительные действия с созданным спрайтом
            // ...

            // Пример: использование спрайта в SpriteRenderer
            GameObject spriteObject = new GameObject("SpriteObject");
            SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
        }

        Debug.Log("Все текстуры преобразованы в Sprite (2D and UI).");
    }
}
