using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the current cursor position of the laser pointer so that it can be retrieved by scripts like the AnnotationManager which rely on the pointing position
/// </summary>
public class PointerCursor : Singleton<PointerCursor> {
    
    /// <summary>
    /// Current position of the cursor, where the user is pointing
    /// </summary>
	public Vector3 HitPosition { get; private set; }

    /// <summary>
    /// Sets the new current hit position
    /// </summary>
    /// <param name="hitPosition"></param>
    public void UpdateHitPosition(Vector3 hitPosition)
    {
        HitPosition = hitPosition;
    }
}
