using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeTokenResponse
{
    public bool success;
    public string message;
    public data data { get; set; }
}

public class data
{
    public string Url { get; set; }
}