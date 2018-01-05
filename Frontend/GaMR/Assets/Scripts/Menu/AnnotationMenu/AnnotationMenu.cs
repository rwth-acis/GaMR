using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnotationMenu : BaseMenu
{
    private FocusableButton editButton, deleteButton, closeButton;
    private TextMesh label;
    private Caption caption;

    private static AnnotationMenu currentlyOpenAnnotationMenu;

    public AnnotationContainer Container { get; set; }

    protected override void Start()
    {
        base.Start();
        InitializeButtons();
        label = transform.Find("Label").GetComponent<TextMesh>();
        caption = transform.Find("TextField").GetComponent<Caption>();

        if (Container != null)
        {
            caption.Text = Container.Annotation.Text;
        }
        else
        {
            caption.Text = "No annotation associated!";
        }

        if (currentlyOpenAnnotationMenu != null)
        {
            currentlyOpenAnnotationMenu.Close();
        }
        currentlyOpenAnnotationMenu = this;
    }

    private void InitializeButtons()
    {
        closeButton = transform.Find("Close Button").GetComponent<FocusableButton>();
        deleteButton = transform.Find("Delete Button").GetComponent<FocusableButton>();
        editButton = transform.Find("Edit Button").GetComponent<FocusableButton>();
    }

    private void Close()
    {
        currentlyOpenAnnotationMenu = null;
        if (Container != null)
        {
            Container.Deselect();
        }
        Destroy(gameObject);
    }

    public override void OnUpdateLanguage()
    {
        base.OnUpdateLanguage();
        closeButton.Text = LocalizationManager.Instance.ResolveString("Close");
        deleteButton.Text = LocalizationManager.Instance.ResolveString("Delete");
        editButton.Text = LocalizationManager.Instance.ResolveString("Edit");

        label.text = LocalizationManager.Instance.ResolveString("Annotation");
    }
}
