using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps track of the target platform and the compatible target backend's version number
/// It also provides an overall version number from which the platform can be identified
/// </summary>
public class VersionManager : Singleton<VersionManager>
{
    /// <summary>
    /// The target platform for the build
    /// </summary>
    [SerializeField]
    private TargetPlatform targetPlatform;

    /// <summary>
    /// The platform specific revision number
    /// This is an addition to the general version number and keeps track of platform-dependent changes
    /// </summary>
    [SerializeField]
    private uint platformRevision;

    [SerializeField]
    [Tooltip("Major version number of the compatible target backend")]
    private uint backendMajor = 1;
    [SerializeField]
    [Tooltip("Minor version number of the compatible target backend")]
    private uint backendMinor;

    /// <summary>
    /// Gives the full version number of the current build
    /// It has the form 1.0.0p-0 where the p is the platform abbreviation and -0 is the number platform specific adaptions in the particular version
    /// </summary>
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
                case TargetPlatform.MOBILE:
                    platformAbbreviation = "m";
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
}

/// <summary>
/// The target platform of the current build
/// </summary>
public enum TargetPlatform
{
    HOLOLENS, DESKTOP, HTC_VIVE, MOBILE
}
