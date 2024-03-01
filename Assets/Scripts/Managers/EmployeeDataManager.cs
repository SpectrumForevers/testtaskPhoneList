using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EmployeeDataManager : MonoBehaviour
{
    [SerializeField] List<ProfileData> _profiles;

    [SerializeField] List<GameObject> litleProfiles;
    [SerializeField] GameObject contentHolder;

    [SerializeField] List<Texture2D> listImages;
    [SerializeField] string jsontext;

    [SerializeField] UserData userData1 = new UserData();
    
    bool first = true;

    private void OnEnable()
    {
        EventBus.listImages += ListImageGet;
        EventBus.json += JsonGet;
    }
    private void OnDisable()
    {
        EventBus.listImages -= ListImageGet;
        EventBus.json -= JsonGet;
    }
    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            litleProfiles.Add(Instantiate(GlobalAccesPoint.instance.pofileLitle, contentHolder.transform));
        }
    }
    private void Update()
    {
        if(first)
        {
            GetRandomProfiles();
        }

       
    }
    private void ListImageGet(List<Texture2D> texture2Ds)
    {
        listImages = texture2Ds;
    }
    private void JsonGet(string json)
    {
        this.jsontext = json;
        ReadJsonFile();

    }
    void ReadJsonFile()
    {
        userData1 = JsonUtility.FromJson<UserData>(jsontext);
    }

    void GetRandomProfiles()
    {
        if(userData1.data.Count > 100)
        {
            for(int i = 0; i < litleProfiles.Count;i++)
            {
                litleProfiles[i].GetComponent<ProfileManager>().user = userData1.data[Random.Range(0, 2000)];
                litleProfiles[i].GetComponent<ProfileManager>().InitProfileData(listImages[i]);
            }
            first = false;
        }
    }

}

[System.Serializable]
public class UserData
{
    public List<User> data;
}

[System.Serializable]
public class User
{
    public int id;
    public string first_name;
    public string last_name;
    public string email;
    public string gender;
    public string ip_address;
}
