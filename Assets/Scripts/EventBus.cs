using System;
using System.Collections.Generic;
using UnityEngine;
public static class EventBus
{
    public static Action<List<Texture2D>> listImages;

    public static Action<String> json;

    public static Action<GameObject> profileLitleAdd;

    public static Action<GameObject> profileLitleDelite;
}
