using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBox : MonoBehaviour
{

    public TextMesh label;
    public GameObject button;
    public MessageBoxType type = MessageBoxType.INFORMATION;
    private string text;
    private Button btn;

    // Use this for initialization
    void Start()
    {
        btn = button.GetComponent<Button>();
        btn.OnPressed = Accept;
    }

    public static void Show(string text, MessageBoxType type)
    {
        GameObject messageBox = (GameObject) Instantiate(Resources.Load("MessageBox"));
        MessageBox msgBox = messageBox.GetComponent<MessageBox>();
        msgBox.Text = text;
        msgBox.type = type;
    }

    public void Accept()
    {
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
