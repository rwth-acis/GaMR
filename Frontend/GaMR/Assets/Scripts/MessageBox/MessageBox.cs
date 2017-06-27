using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBox : MonoBehaviour
{

    public TextMesh label;
    public GameObject button;
    public MessageBoxType type = MessageBoxType.INFORMATION;
    public Transform iconPosition;
    private string text;
    private Button btn;
    private static int count = 0;
    private SimpleTagalong tagalongScript;

    // Use this for initialization
    void Start()
    {
        btn = button.GetComponent<Button>();
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

    public static void Show(string text, MessageBoxType type)
    {
        GameObject messageBox = (GameObject) Instantiate(Resources.Load("MessageBox"));
        MessageBox msgBox = messageBox.GetComponent<MessageBox>();
        msgBox.Text = text;
        msgBox.type = type;
        count++;
    }

    public void Accept()
    {
        count--;
        Destroy(gameObject);
    }

    public string Text
    {
        get { return text; }
        set { text = value; label.text = value; }
    }
}

public enum MessageBoxType
{
    SUCCESS, INFORMATION, ERROR, WARNING
}
