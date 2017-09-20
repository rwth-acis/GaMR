using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Tests;
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ModelSynchronizer : Singleton<ModelSynchronizer>
{
    public Transform worldAnchor;

    private void Start()
    {
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.ModelSpawn] = RemoteModelSpawned;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.ModelDelete] = OnRemoteDestroy;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.BoundingBoxTransform] = ReceivedRemoteTransformChange;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.AnnotationsUpdated] = OnAnnotationsUpdated;
    }

    private void OnAnnotationsUpdated(NetworkInMessage msg)
    {
        Debug.Log("Broadcasting annotations update");
        worldAnchor.BroadcastMessage("RemoteAnnotationsUpdated", msg);
        
    }

    public void LoadModelForAll(string modelName)
    {
        Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
        Vector3 localSpawnPosition = worldAnchor.InverseTransformPoint(spawnPosition); // get coordinates in world-anchor-local space

        BoundingBoxId id = new BoundingBoxId();

        CustomMessages.Instance.SendModelSpawn(modelName, id, localSpawnPosition); // broadcast spawn event

        ModelLoadManager manager = new ModelLoadManager(localSpawnPosition, worldAnchor, id, false);

        manager.Load(modelName); // load model
    }

    public void RemoteModelSpawned(NetworkInMessage msg)
    {
        Debug.Log("Received remote model spawn");
        long userId = msg.ReadInt64();
        if (userId != SharingStage.Instance.Manager.GetLocalUser().GetID())
        {
            string modelName = msg.ReadString();
            int localBoxId = msg.ReadInt32();
            BoundingBoxId boundingBoxId = new BoundingBoxId(userId, localBoxId);
            Vector3 spawnPosition = CustomMessages.Instance.ReadVector3(msg);
            ModelLoadManager manager = new ModelLoadManager(spawnPosition, worldAnchor, boundingBoxId, true);
            manager.Load(modelName);
        }
    }

    private void OnRemoteDestroy(NetworkInMessage msg)
    {
        long userId = msg.ReadInt64();
        if (userId != SharingStage.Instance.Manager.GetLocalUser().GetID())
        {
            long boxUser = msg.ReadInt64();
            int boxNumber = msg.ReadInt32();
            BoundingBoxId boundingBoxId = new BoundingBoxId(boxUser, boxNumber);
            Debug.Log("Received remote destroy for " + boundingBoxId.ToString());
            if (TransformationManager.instances.ContainsKey(boundingBoxId.ToString()))
            {
                BoundingBoxActions actions = TransformationManager.instances[boundingBoxId.ToString()].gameObject.GetComponent<BoundingBoxActions>();
                actions.DeleteLocalObject();
            }
            else
            {
                Debug.Log("Delete command for unknown bounding box: " + boundingBoxId.ToString());
            }
        }
    }

    private static void ReceivedRemoteTransformChange(NetworkInMessage msg)
    {
        long userId = msg.ReadInt64(); // this is the user ID
        long boxUser = msg.ReadInt64();
        int localBoundingBoxId = msg.ReadInt32();
        BoundingBoxId boundingBoxId = new BoundingBoxId(boxUser, localBoundingBoxId);
        Vector3 newPosition = CustomMessages.Instance.ReadVector3(msg);
        Quaternion newRotation = CustomMessages.Instance.ReadQuaternion(msg);
        Vector3 newScale = CustomMessages.Instance.ReadVector3(msg);

        if (TransformationManager.instances.ContainsKey(boundingBoxId.ToString()))
        {
            TransformationManager.instances[boundingBoxId.ToString()].OnRemoteTransformChanged(newPosition, newRotation, newScale);
        }
        else
        {
            Debug.LogError("Received transform from an unknown bounding box: " + boundingBoxId.ToString());
        }
    }
}
