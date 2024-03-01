using UnityEngine;

using UnityEngine.UI;

[CreateAssetMenu(fileName = "ProfileData", menuName = "ScriptableObjects/ProfileData", order = 1)]
public class ProfileData : ScriptableObject
{
    public Sprite profileImage;

    public string profileName;
    public string profileGender;
    public string profileEmail;
    public string profileAdress;

    public FavoriteCheck profileFavoriteCheck;

}

public enum FavoriteCheck
{
    NonFavorite,
    Favorite,
}