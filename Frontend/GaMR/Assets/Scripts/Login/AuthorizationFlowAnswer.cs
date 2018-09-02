using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AuthorizationFlowAnswer
{
    public string access_token;
    public string token_type;
    public int expires_in;
    public string scope;
    public string id_token;
    public string error;
    public string error_description;
}
