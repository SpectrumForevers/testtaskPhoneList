using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
public class ProfileManager : MonoBehaviour
{
    [SerializeField] ProfileData profileData;

    [SerializeField] Image favoriteImage;

    [SerializeField] Image profileImage;

    [SerializeField] TMP_Text profileName;
    [SerializeField] TMP_Text profileGender;
    [SerializeField] TMP_Text profileAdress;
    [SerializeField] TMP_Text profileEmail;

    private void Awake()
    {
        if (profileData != null)
        {
            InitProfile();
        }
    }
    void InitProfile()
    {
        if(profileData.profileImage != null)
        {
            profileImage = profileData.profileImage;
        }
        if (profileData.profileImage == null)
        {
            profileImage.sprite = GlobalAccesPoint.instance.basicProfileImage;
        }

        switch (profileData.profileFavoriteCheck)
        {
            case FavoriteCheck.NonFavorite:
                favoriteImage.sprite = GlobalAccesPoint.instance.favoriteUncheck;
                break;

            case FavoriteCheck.Favorite:
                favoriteImage.sprite = GlobalAccesPoint.instance.favoriteCheck;
                break;
        }
        profileName.text = profileData.profileName;
        profileGender.text = profileData.profileGender;
        profileAdress.text = profileData.profileAdress;
        profileEmail.text = profileData.profileEmail;
    }
}
