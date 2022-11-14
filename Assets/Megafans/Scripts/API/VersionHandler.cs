using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
    public string CurrentIosVersion;
    public string CurrentAndroidVersion;
}

[System.Serializable]
public class VersionHandler 
{
    public bool success;
    public string message;
    public Data data = new Data();
}