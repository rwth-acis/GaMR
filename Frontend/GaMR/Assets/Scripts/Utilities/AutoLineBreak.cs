using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class AutoLineBreak {

	public static string StringWithLineBreaks(TextMesh textMesh, string text, float maxWidth)
    {
        string originalText = textMesh.text;
        // line breaks if the last char was not a letter
        string[] words = Regex.Split(text, @"(?<=[.,;+\- \n])");

        string res = "";
        string currentLine = "";

        foreach(string word in words)
        {
            textMesh.text = currentLine + word;
            if (Geometry.GetBoundsIndependentFromRotation(textMesh.transform).size.x <= maxWidth)
            {
                currentLine += word;
            }
            else
            {
                res += currentLine + Environment.NewLine;
                currentLine = word;
            }

            if (word.EndsWith("\n"))
            {
                res += currentLine;
                currentLine = "";
            }
        }

        res += currentLine;

        // in case the text could not be wrapped and exceeds the dimensions: still display it
        if (res == "")
        {
            res = text;
        }

        textMesh.text = originalText;
        return res;
    }
}
