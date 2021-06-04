using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace i5.GaMR.Tests.EditMode
{
    public static class EditModeTestHelper
    {
        public static void LoadTestScene()
        {
            EditorSceneManager.OpenScene("Assets/GaMR/Tests/EditMode/EditTestScene.unity");
        }
    }
}
