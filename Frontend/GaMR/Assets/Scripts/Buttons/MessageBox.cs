using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements all logic of a MessageBox which can be displayed to the user for notification
/// </summary>
public class MessageBox : MonoBehaviour
{
    /// <summary>
    /// The label of the MessageBox
    /// </summary>
    public TextMesh label;
    /// <summary>
    /// The GameObject-button on the MessageBox
    /// </summary>
    public GameObject button;
    /// <summary>
    /// Specifies the type of the MessageBox; this influences which icon will be shown
    /// </summary>
    public MessageBoxType type = MessageBoxType.INFORMATION;
    /// <summary>
    /// Transform which corresponds to the position where the icon will be displayed
    /// </summary>
    public Transform iconPosition;
    /// <summary>
    /// The text which should be shown to the user
    /// </summary>
    private string text;
    /// <summary>
    /// The Button component on the button-gameobject
    /// </summary>
    private GaMRButton btn;
    /// <summary>
    /// The number of currently opened MessageBoxes
    /// This is used in order to display multiple MessageBoxes behind each other
    /// </summary>
    private static int count = 0;
    /// <summary>
    /// The component which lets the MessageBox hover in the user's view
    /// It is needed to set the depth at which it will be displayed
    /// </summary>
    private SimpleTagalong tagalongScript;

    private float maxWidth;
    private Transform background;


    private void Awake()
    {
        background = transform.Find("Background");

        float backgroundWidth = Geometry.GetBoundsIndependentFromRotation(background).size.z;

        maxWidth = backgroundWidth - (backgroundWidth/2f - (background.localPosition.x - label.transform.localPosition.x));
    }

    /// <summary>
    /// Initializes the MessageBox: gets all necessary components
    /// and creates the icon which corresponds to the type of the MessageBox
    /// </summary>
    void Start()
    {
        btn = button.GetComponent<GaMRButton>();
        tagalongScript = GetComponent<SimpleTagalong>();

        btn.OnPressed = Accept;
        if (type == MessageBoxType.ERROR)
        {
            Instantiate(Resources.Load("Animated Error"),iconPosition.position, Quaternion.identity, iconPosition);
        }
        else if (type == MessageBoxType.SUCCESS)
        {
            Instantiate(Resources.Load("Animated Success"), iconPosition.position, Quaternion.identity, iconPosition);
        }
        else if (type == MessageBoxType.WARNING)
        {
            Instantiate(Resources.Load("Animated Warning"), iconPosition.position, Quaternion.identity, iconPosition);
        }
        else if (type == MessageBoxType.INFORMATION)
        {
            Instantiate(Resources.Load("Animated Info"), iconPosition.position, Quaternion.identity, iconPosition);
        }

        tagalongScript.TagalongDistance = 1.7f + 0.1f * count;
    }

    /// <summary>
    /// Call this to show a new MessageBox
    /// </summary>
    /// <param name="text">The text message which should be displayed to the user</param>
    /// <param name="type">The type of the MessageBox</param>
    public static void Show(string text, MessageBoxType type)
    {
        // load the MessageBox from the resources and set the necessary variables
        GameObject messageBox = (GameObject) Instantiate(Resources.Load("MessageBox"));
        messageBox.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3;
        MessageBox msgBox = messageBox.GetComponent<MessageBox>();
        msgBox.Text = text;
        msgBox.type = type;
        count++;
    }

    /// <summary>
    /// This is called when the user clicks on the provided button
    /// It destroys the MessageBox
    /// </summary>
    public void Accept()
    {
        count--;
        Destroy(gameObject);
    }

    /// <summary>
    /// The text message which should be shown to the user
    /// automatically updates the text-view on the MessageBox
    /// </summary>
    public string Text
    {
        get { return text; }
        //set { text = value; label.text = value; }
        set { text = AutoLineBreak.StringWithLineBreaks(label, value, maxWidth); label.text = text; }
    }
}

/// <summary>
/// Possible types of MessageBoxes
/// </summary>
public enum MessageBoxType
{
    SUCCESS, INFORMATION, ERROR, WARNING
}
