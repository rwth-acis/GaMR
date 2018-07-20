using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Utilities for line breaks
/// </summary>
public class AutoLineBreak {

    /// <summary>
    /// Automatically inserts line breaks in a given text to fit the text to the given textMesh and width
    /// </summary>
    /// <param name="textMesh">The textMesh which provides the font settings</param>
    /// <param name="text">The base text to which line breaks should be applied</param>
    /// <param name="maxWidth">The maximum allowed width of the text in the textMesh</param>
    /// <returns>The base text with line breaks</returns>
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
