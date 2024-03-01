using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalAccesPoint : MonoBehaviour
{
    public static GlobalAccesPoint instance;

    public Sprite favoriteCheck;
    public Sprite favoriteUncheck;

    public Sprite basicProfileImage;

    public string fileDataName;

    public GameObject profile;
    public GameObject pofileLitle;
    

    private void Awake()
    {
        instance = this;
    }
}
