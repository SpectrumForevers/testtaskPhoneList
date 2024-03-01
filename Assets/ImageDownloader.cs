using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ImageDownloader : MonoBehaviour
{
    // ������ �� �����������
    private string[] imageUrls = { "https://picsum.photos/200", "https://picsum.photos/201", "https://picsum.photos/202" };

    // ������ ��� �������� ��������� �������
    [SerializeField] private List<Texture2D> downloadedTextures = new List<Texture2D>();

    // ����� ��� ��������, ���������� � ��������� ���� ��������
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

    // �������� ��� ����������� �������� �����������
    private IEnumerator DownloadImagesCoroutine()
    {
        foreach (string imageUrl in imageUrls)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl))
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

                    // ��������� ���� � ����� Resources
                    string resourcesPath = "Assets/Resources/DownloadedImages";

                    // ���������� ����������� � ����� Resources
                    SaveTextureToFile(texture, Path.Combine(resourcesPath, filename));

                    // ��������� �������������� ���� � �������� � ����� Resources
                    string relativePath = Path.Combine("Assets/Resources/DownloadedImages", filename);

                    // ��������� ���� �������� �� Sprite (2D and UI)
                    SetTextureImporterSettings(relativePath);

                    // ���������� �������� � ������
                    downloadedTextures.Add(texture);
                }
                else
                {
                    // ��������� ������ ��� �������� �����������
                    Debug.LogError("������ �������� �����������: " + www.error);
                }
            }
        }

        // ����� ������ ��� �������������� ������� � Sprite (2D and UI), ����� ���� ��� ���� ��������� ��������
        //ConvertTexturesToSprites();
    }

    // ����� ��� ��������� ����������� ����� �����
    private string GenerateUniqueFilename(string baseName, string extension)
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        string guid = System.Guid.NewGuid().ToString("N");
        return $"{baseName}_{timestamp}_{guid}.{extension}";
    }

    // ����� ��� ���������� �������� � ����
    private void SaveTextureToFile(Texture2D texture, string filePath)
    {
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("����������� ��������� � ����: " + filePath);
    }

    // ����� ��� ��������� ���� �������� �� Sprite (2D and UI)
    private void SetTextureImporterSettings(string relativePath)
    {
        // ����������� ���� � �������� � ��������� �������� Unity
        string assetPath = relativePath.Substring(0, relativePath.LastIndexOf('.'));

        // �������� ��������
        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

        // �������� ������� �������� � ��������� ��������
        if (textureImporter != null)
        {
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spritePixelsPerUnit = 100; // ������� ������ ��������
            textureImporter.filterMode = FilterMode.Bilinear;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed; // ��� ������ ����� ������, � ����������� �� ������ ������
            textureImporter.SaveAndReimport();
            Debug.Log("��� �������� ������� �� Sprite (2D and UI): " + assetPath);
        }
        else
        {
            Debug.LogError("������ ��� ��������� ���� ��������. �������� �� �������: " + assetPath);
        }
    }

    // ����� ��� �������������� ������� � Sprite (2D and UI)
    private void ConvertTexturesToSprites()
    {
        foreach (Texture2D texture in downloadedTextures)
        {
            // ���� �������������� �������� � ���������
            // ...

            // ������: �������������� � Sprite (2D and UI)
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            // ���� �������������� �������� � ��������� ��������
            // ...

            // ������: ������������� ������� � SpriteRenderer
            GameObject spriteObject = new GameObject("SpriteObject");
            SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
        }

        Debug.Log("��� �������� ������������� � Sprite (2D and UI).");
    }
}
