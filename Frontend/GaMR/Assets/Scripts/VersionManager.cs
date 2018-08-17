using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionManager : Singleton<VersionManager>
{
    [SerializeField]
    private uint majorRelease = 1;
    [SerializeField]
    private uint minorRelease = 0;
    [SerializeField]
    private uint patchLevel = 0;

    public uint MajorRelease { get { return majorRelease; } }

    public uint MinorRelease { get { return minorRelease; } }

    public uint PatchLevel { get { return patchLevel; } }

    public string VersionString { get { return MajorRelease + "." + MinorRelease + "." + PatchLevel; } }
}
