using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
public class Profile : MonoBehaviour
{
    public GameObject profileLitle;

    public ProfileData profileData;

    [SerializeField] GameObject closeButton;
    
    [SerializeField] Image favoriteImage;

    [SerializeField] Image profileImage;

    [SerializeField] TMP_Text profileName;
    [SerializeField] TMP_Text profileGender;
    [SerializeField] TMP_Text profileAdress;
    [SerializeField] TMP_Text profileEmail;

    [HideInInspector] public GameObject EmpoyeeData;
    private void Start()
    {
        if (profileData != null)
        {
            InitProfile();
        }
        closeButton.GetComponent<Button>().onClick.AddListener(delegate { CloseProfile(); });
    }
    private void CloseProfile()
    {
        EmpoyeeData.transform.GetChild(0).gameObject.SetActive(true);
        Destroy(gameObject);
    }
    void InitProfile()
    {
        if(profileData.profileImage != null)
        {
            profileImage.sprite = profileData.profileImage;
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
