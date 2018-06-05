using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GoogleAccessTokenResponse
{
    public string access_token;
    public int expires_in;
    public string token_type;
    public string refresh_token;
    public string error;
    public string error_description;
}
