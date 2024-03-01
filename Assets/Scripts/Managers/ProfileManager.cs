using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [HideInInspector] public User user;
    [HideInInspector] public Sprite image;
    private void Awake()
    {
        //InitProfileData();
    }
    private void OpenProfile()
    {

    }

    public void FavoriteSwitch()
    {
        if (profileData.profileFavoriteCheck == FavoriteCheck.Favorite)
        {
            profileData.profileFavoriteCheck = FavoriteCheck.NonFavorite;

        }
        if (profileData.profileFavoriteCheck == FavoriteCheck.NonFavorite)
        {
            profileData.profileFavoriteCheck = FavoriteCheck.Favorite;
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
