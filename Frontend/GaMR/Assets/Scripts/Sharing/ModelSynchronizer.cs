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
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.ModelSpawn] = RemoteModelSpawned;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.ModelDelete] = OnRemoteDestroy;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.BoundingBoxTransform] = ReceivedRemoteTransformChange;
    }


    public void LoadModelForAll(string modelName)
    {
        Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
        Vector3 localSpawnPosition = worldAnchor.InverseTransformPoint(spawnPosition); // get coordinates in world-anchor-local space
        CustomMessages.Instance.SendModelSpawn(modelName, localSpawnPosition); // broadcast spawn event
        ModelLoadManager.Instance.spawnPosition = localSpawnPosition;
        ModelLoadManager.Instance.Load(modelName); // load model
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

    private void OnRemoteDestroy(NetworkInMessage msg)
    {
        long userId = msg.ReadInt64();
        if (userId != SharingStage.Instance.Manager.GetLocalUser().GetID())
        {
            int boxNumber = msg.ReadInt32();
            if (TransformationManager.instances.ContainsKey(boxNumber))
            {
                Destroy(TransformationManager.instances[boxNumber].gameObject);
            }
        }
    }

    private static void ReceivedRemoteTransformChange(NetworkInMessage msg)
    {
        msg.ReadInt64(); // this is the user ID
        int msgBoundingBoxId = msg.ReadInt32();
        Vector3 newPosition = CustomMessages.Instance.ReadVector3(msg);
        Quaternion newRotation = CustomMessages.Instance.ReadQuaternion(msg);
        Vector3 newScale = CustomMessages.Instance.ReadVector3(msg);

        if (TransformationManager.instances.ContainsKey(msgBoundingBoxId))
        {
            TransformationManager.instances[msgBoundingBoxId].OnRemoteTransformChanged(newPosition, newRotation, newScale);
        }
        else
        {
            Debug.LogError("Received transform from an unknown bounding box");
        }
    }
}
