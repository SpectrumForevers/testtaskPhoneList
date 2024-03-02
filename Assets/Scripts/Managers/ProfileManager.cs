using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] GameObject imageProfile;
    [SerializeField] GameObject buttonOpenProfile;
    [SerializeField] GameObject favorite;
    [SerializeField] ProfileData profileData;
    
    [SerializeField] TMP_Text nameProfile;
    [SerializeField] TMP_Text mail;
    [SerializeField] TMP_Text ipAddress;

    public GameObject EmpoyeeData;
    [HideInInspector] public User user;
    [HideInInspector] public Sprite image;
    private void Awake()
    {
        //InitProfileData();
    }
    private void Start()
    {
        buttonOpenProfile.GetComponent<Button>().onClick.AddListener(delegate { OpenProfile(); }) ;
        favorite.GetComponent<Button>().onClick.AddListener(delegate { FavoriteSwitch(); });
    }

    private void OpenProfile()
    {
        GameObject bufer = new GameObject();
        bufer = Instantiate(GlobalAccesPoint.instance.profile, EmpoyeeData.transform);
        bufer.GetComponent<Profile>().profileData = profileData;
        bufer.GetComponent<Profile>().profileLitle = gameObject;
        bufer.GetComponent<Profile>().EmpoyeeData = EmpoyeeData;
        EmpoyeeData.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void FavoriteSwitch()
    {
        if (profileData.profileFavoriteCheck == FavoriteCheck.Favorite)
        {
            favorite.GetComponent<Image>().sprite = GlobalAccesPoint.instance.favoriteUncheck;
            profileData.profileFavoriteCheck = FavoriteCheck.NonFavorite;
            EventBus.profileLitleDelite?.Invoke(gameObject);
            return;
        }
        else
        {
            favorite.GetComponent<Image>().sprite = GlobalAccesPoint.instance.favoriteCheck;
            profileData.profileFavoriteCheck = FavoriteCheck.Favorite;
            EventBus.profileLitleAdd?.Invoke(gameObject);
            return;
        }
    }
    public void InitProfileData(Texture2D texture2D)
    {
        profileData = ScriptableObject.CreateInstance<ProfileData>();
        image = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
        imageProfile.GetComponent<Image>().sprite = image;
        profileData.profileImage = image;

        nameProfile.text =  user.first_name + " " + user.last_name;
        profileData.profileName = nameProfile.text;

        mail.text = user.email;
        profileData.profileEmail = mail.text;

        ipAddress.text = user.ip_address;
        profileData.profileAdress = ipAddress.text;

        profileData.profileGender = user.gender;
    }
}
