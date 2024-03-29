using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FavoriteManager : MonoBehaviour
{
    [SerializeField] List<GameObject> profileLitle;
    [SerializeField] private GameObject content;
    [SerializeField] GameObject exitButton;
    [SerializeField] GameObject enableEmployeedataBuf;
    private void OnEnable()
    {
        EventBus.profileLitleAdd += AddProfile;
        EventBus.profileLitleDelite += DeliteProfile;
    }

    private void OnDisable()
    {
        EventBus.profileLitleAdd -= AddProfile;
        EventBus.profileLitleDelite -= DeliteProfile;
    }
    private void Start()
    {
        exitButton.GetComponent<Button>().onClick.AddListener(delegate { ActivateDataEmpl(); });
    }
    private void ActivateDataEmpl()
    {
        enableEmployeedataBuf.SetActive(true);
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    private void AddProfile(GameObject profile)
    {
        GameObject buf = Instantiate(profile, content.transform);
        profileLitle.Add(buf);
    }

    private void DeliteProfile(GameObject profile)
    {
        GameObject buf = null;
        
        for (int i = profileLitle.Count-1; i >= 0; i--)
        {
            
            if (profileLitle[i].GetComponent<ProfileManager>().profileData.idProfile == profile.GetComponent<ProfileManager>().profileData.idProfile)
            {
                buf = profileLitle[i];
                
            }
        }
        profileLitle.Remove(buf);
        Destroy(buf);
    }
}
