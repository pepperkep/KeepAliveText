using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI batteryText;
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color correctColor;
    [SerializeField] private Color wrongColor;
    [SerializeField] private AnimationCurve brightnessChange;
    [SerializeField] private GameObject endUI;

    public string Text
    {
        get => mainText.text;
    }

    public void SetBatteryText(float batteryValue)
    {
        float roundedValue = Mathf.Round(batteryValue * 10) / 10;
        string nextBatteryText = "" + roundedValue;
        if(roundedValue % 1 == 0)
            nextBatteryText += ".0";
        nextBatteryText += "%";
        batteryText.text = nextBatteryText;
    }

    public void UpdateCharacterColor(int characterIndex, int correctnessValue, float battery)
    {
        int materialIndex = mainText.textInfo.characterInfo[characterIndex].materialReferenceIndex;
        Color32[] nextColors = mainText.textInfo.meshInfo[materialIndex].colors32;
        int vertexIndex = mainText.textInfo.characterInfo[characterIndex].vertexIndex;

        if(mainText.textInfo.characterInfo[characterIndex].character == 32 || !mainText.textInfo.characterInfo[characterIndex].isVisible)
            return;

        Color32 nextColor;
        switch(correctnessValue)
        {
            case 1:
                nextColor = ChangeBrightness(correctColor, brightnessChange.Evaluate(battery));
                break;
            case 2:
                nextColor = ChangeBrightness(wrongColor, brightnessChange.Evaluate(battery));
                break;
            default:
                nextColor = ChangeBrightness(defaultColor, brightnessChange.Evaluate(battery));
                break;
        }

        for(int j = 0; j < 4; j++)
        {
            nextColors[vertexIndex + j] = nextColor;
        }
        mainText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    /*
        Correctness values: 0 none, 1 correct, 2 incorrect
     */
    public void UpdateScreenText(string screenText, int[] correctnessValues, float battery, List<int> columnsToSwap = null)
    {
        mainText.text = screenText;
        mainText.ForceMeshUpdate();
        if(columnsToSwap != null)
        {
            for(int i = 0; i < columnsToSwap.Count; i++)
            {
                SwapTextColumns(columnsToSwap[i]);
            }
        }
        int characterCount = mainText.textInfo.characterCount;
        int materialIndex = mainText.textInfo.characterInfo[0].materialReferenceIndex;
        Color32[] nextColors = mainText.textInfo.meshInfo[materialIndex].colors32;
        for(int i = 0; i < characterCount; i++)
        {
            int vertexIndex = mainText.textInfo.characterInfo[i].vertexIndex;

            if(mainText.textInfo.characterInfo[i].character == 32 || !mainText.textInfo.characterInfo[i].isVisible)
                continue;

            Color32 nextColor;
            if(i < correctnessValues.Length)
            {
                switch(correctnessValues[i])
                {
                    case 1:
                        nextColor = ChangeBrightness(correctColor, brightnessChange.Evaluate(battery));
                        break;
                    case 2:
                        nextColor = ChangeBrightness(wrongColor, brightnessChange.Evaluate(battery));
                        break;
                    default:
                        nextColor = ChangeBrightness(defaultColor, brightnessChange.Evaluate(battery));
                        break;
                }
            }
            else
            {
                nextColor = ChangeBrightness(defaultColor, brightnessChange.Evaluate(battery));
            }

            for(int j = 0; j < 4 && vertexIndex < nextColors.Length; j++)
            {
                nextColors[vertexIndex + j] = nextColor;
            }
        }
        mainText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    public void GameEnd()
    {
        batteryText.gameObject.SetActive(false);
        mainText.gameObject.SetActive(false);
        endUI.SetActive(true);
    }

    private void SwapTextColumns(int columnToSwap)
    {
        List<string> textByLine = new List<string>();
        char nextChar = mainText.text[mainText.textInfo.lineInfo[0].firstVisibleCharacterIndex + columnToSwap];
        char temp;
        for(int i = 1; i < mainText.textInfo.lineInfo.Length; i++)
        {
            int lineStart = mainText.textInfo.lineInfo[i].firstCharacterIndex;
            int lineEnd = mainText.textInfo.lineInfo[i].lastCharacterIndex;
            if(lineEnd - (lineStart + columnToSwap + 1) > 0 && lineStart + columnToSwap + 1 < mainText.text.Length)
            {
                temp = mainText.text[mainText.textInfo.lineInfo[i].firstVisibleCharacterIndex + columnToSwap];
                if(temp != '\n')
                {
                    if(lineStart + columnToSwap != lineEnd)
                        textByLine.Add(mainText.text.Substring(lineStart, columnToSwap) + nextChar
                            + mainText.text.Substring(lineStart + columnToSwap + 1, lineEnd - (lineStart + columnToSwap)));
                    else
                        textByLine.Add(mainText.text.Substring(lineStart, columnToSwap) + nextChar);
                    nextChar = temp;
                }
                else
                {
                    textByLine.Add(mainText.text.Substring(lineStart, lineEnd - lineStart));
                }
            }
            else
            {
                if(lineEnd < mainText.text.Length)
                    textByLine.Add(mainText.text.Substring(lineStart, lineEnd - lineStart));
            }
        }
        int firstlineStart = mainText.textInfo.lineInfo[0].firstVisibleCharacterIndex;
        int firstlineEnd = mainText.textInfo.lineInfo[0].lastCharacterIndex;
        if(firstlineEnd - (firstlineStart + columnToSwap + 1) > 0)
        {
            if(firstlineStart + columnToSwap != firstlineEnd)
                textByLine.Insert(0, mainText.text.Substring(firstlineStart, columnToSwap) + nextChar
                    + mainText.text.Substring(firstlineStart + columnToSwap + 1, firstlineEnd - (firstlineStart + columnToSwap)));
            else
                textByLine.Insert(0, mainText.text.Substring(firstlineStart, columnToSwap) + nextChar);
        }
        else
        {
            textByLine.Add(mainText.text.Substring(firstlineStart, firstlineEnd - firstlineStart));
        }
        string nextText = "";
        foreach(string line in textByLine)
        {
            nextText += line;
        }
        mainText.text = nextText;
        mainText.ForceMeshUpdate();
    }

    private Color ChangeBrightness(Color originalColor, float brightnessLevel)
    {
        float H, S, V;
        Color.RGBToHSV(originalColor, out H, out S, out V);
        return Color.HSVToRGB(H, S, V * brightnessLevel / 100);
    }
}
