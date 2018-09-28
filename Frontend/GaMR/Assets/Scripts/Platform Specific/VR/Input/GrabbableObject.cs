using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour, IGrabbable
{
    public bool scalable = true;
    public bool useGravity = false;

    private Rigidbody grabbableRigidBody;
    private Transform originalParent;
    private List<VRInputManager> influencingInputManagers;
    private float lastDistance = 0;
    private bool firstScaleFrame = true;
    private bool ispersistent = false;

    public bool IsGrabbed
    {
        // grabbed if it is held by at least one controller (this also includes scale mode)
        get { return influencingInputManagers.Count > 0; }
    }

    public bool IsInScaleMode
    {
        // is in scale mode if it is scalable and if it is currently held by two controllers
        get { return scalable && influencingInputManagers.Count == 2; }
    }

    private void Awake()
    {
        grabbableRigidBody = GetComponent<Rigidbody>();
        influencingInputManagers = new List<VRInputManager>();
        if (grabbableRigidBody == null)
        {
            Debug.LogWarning(gameObject.name + " is grabbable but does not have a rigidbody");
        }
    }

    private void Start()
    {
        ispersistent = gameObject.scene.name == "DontDestroyOnLoad";
    }

    public void OnGrabStarted(VRInputManager sender)
    {
        Debug.Log("OnGrabStarted" + gameObject.name);
        if (grabbableRigidBody != null)
        {
            grabbableRigidBody.isKinematic = true;
            grabbableRigidBody.useGravity = false;
        }

        if (!influencingInputManagers.Contains(sender))
        {
            influencingInputManagers.Add(sender);
        }

        if (influencingInputManagers.Count == 1)
        {
            originalParent = transform.parent;
            transform.parent = sender.transform;
        }
        else if (influencingInputManagers.Count == 2)
        {
            lastDistance = 0;
            firstScaleFrame = true;
            ScalingPivotManager.Instance.Pivot.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            transform.parent = ScalingPivotManager.Instance.Pivot.transform;
        }
    }

    public void OnGrabUpdate(VRInputManager sender)
    {
        // parenting implicitly hanlde position and rotation
        // only handle scale mode
        if (IsInScaleMode)
        {
            if (!firstScaleFrame)
            {
                float distance = Vector3.Magnitude(influencingInputManagers[1].transform.position - influencingInputManagers[0].transform.position);
                if (lastDistance != 0)
                {
                    float factor = distance / lastDistance;
                    Vector3 newScale = factor * ScalingPivotManager.Instance.Pivot.transform.localScale;
                    ScalingPivotManager.Instance.Pivot.transform.localScale = newScale;
                }
            }
            lastDistance = Vector3.Magnitude(influencingInputManagers[1].transform.position - influencingInputManagers[0].transform.position);
            firstScaleFrame = false;
        }
    }

    public void OnGrabCompleted(VRInputManager sender)
    {
        bool wasScaleMode = IsInScaleMode;
        influencingInputManagers.Remove(sender);

        if (wasScaleMode) // now just grab mode
        {
            // set parent from pivot to remaining controller
            transform.parent = influencingInputManagers[0].transform;
        }
        else // completely released now
        {
            if (grabbableRigidBody != null && useGravity) // determine gravity
            {
                grabbableRigidBody.velocity = sender.Controller.velocity;
                grabbableRigidBody.angularVelocity = sender.Controller.angularVelocity;
                grabbableRigidBody.useGravity = useGravity;
                grabbableRigidBody.isKinematic = !useGravity;
            }
            // reset parent to original
            transform.parent = originalParent;
            originalParent = null;

            if (!ispersistent)
            {
                if (GetComponent<ManuallyDestroy>() == null)
                {
                    gameObject.AddComponent<ManuallyDestroy>();
                }
            }
        }
    }
}
