using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Tests;
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSynchronizer : Singleton<ModelSynchronizer>
{
    public Transform worldAnchor;

    private void Start()
    {
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.ModelSpawn] = this.RemoteModelSpawned;
    }


    public void LoadModelForAll(string modelName)
    {
        Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
        ModelLoadManager.Instance.spawnPosition = spawnPosition;
        Vector3 localSpawnPosition = spawnPosition - worldAnchor.position;
        CustomMessages.Instance.SendModelSpawn(modelName, spawnPosition);
        ModelLoadManager.Instance.Load(modelName);
    }

    public void RemoteModelSpawned(NetworkInMessage msg)
    {
        long userId = msg.ReadInt64();
        if (userId != SharingStage.Instance.Manager.GetLocalUser().GetID())
        {
            string modelName = msg.ReadString();
            Vector3 spawnPosition = CustomMessages.Instance.ReadVector3(msg);
            ModelLoadManager.Instance.spawnPosition = spawnPosition;
            ModelLoadManager.Instance.Load(modelName);
        }
    }
}
