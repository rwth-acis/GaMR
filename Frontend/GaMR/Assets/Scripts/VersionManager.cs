using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionManager : Singleton<VersionManager>
{
    [SerializeField]
    private TargetPlatform targetPlatform;

    [SerializeField]
    private uint platformRevision;

    [SerializeField]
    [Tooltip("Major version number of the compatible target backend")]
    private uint backendMajor = 1;
    [SerializeField]
    [Tooltip("Minor version number of the compatible target backend")]
    private uint backendMinor;

    public string VersionNumber
    {
        get
        {
            string platformAbbreviation;
            switch (targetPlatform)
            {
                case TargetPlatform.HOLOLENS:
                    platformAbbreviation = "h";
                    break;
                case TargetPlatform.DESKTOP:
                    platformAbbreviation = "d";
                    break;
                case TargetPlatform.HTC_VIVE:
                    platformAbbreviation = "v";
                    break;
                default:
                    platformAbbreviation = "err";
                    break;
            }

            if (platformRevision > 0)
            {
                return Application.version + platformAbbreviation + "-" + platformRevision;
            }
            else
            {
                return Application.version + platformAbbreviation;
            }
        }
    }

    public TargetPlatform TargetPlatform
    {
        get { return targetPlatform; }
    }
}

public enum TargetPlatform
{
    HOLOLENS, DESKTOP, HTC_VIVE
}
