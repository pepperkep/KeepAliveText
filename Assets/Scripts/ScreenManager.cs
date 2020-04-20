using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI batteryText;
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color correctColor;
    [SerializeField] private Color wrongColor;
    [SerializeField] private AnimationCurve brightnessChange;
    [SerializeField] private GameObject endUI;
    [SerializeField] private float wrongOffset = -0.01f;

    public string Text
    {
        get => mainText.text;
    }

    public void SetBatteryText(float batteryValue)
    {
        float roundedValue = Mathf.Round(batteryValue * 10) / 10;
        string nextBatteryText;
        if(batteryValue == 100)
        {
            nextBatteryText = "100%";
        }
        else
        {
            nextBatteryText = "" + roundedValue;
            if(roundedValue % 1 == 0)
                nextBatteryText += ".0";
            nextBatteryText += "%";
        }
        batteryText.text = nextBatteryText;
    }

    public void UpdateCharacterColor(int characterIndex, int correctnessValue, float battery)
    {
        int materialIndex = mainText.textInfo.characterInfo[characterIndex].materialReferenceIndex;
        Color32[] nextColors = mainText.textInfo.meshInfo[materialIndex].colors32;
        Vector3[] vertices = mainText.textInfo.meshInfo[materialIndex].vertices;
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
            if(correctnessValue == 2)
            {
                vertices[vertexIndex + j].y += wrongOffset;
            }
        }
        if(correctnessValue == 2)
        {
            mainText.textInfo.meshInfo[materialIndex].mesh.vertices = mainText.textInfo.meshInfo[materialIndex].vertices;
            mainText.UpdateGeometry(mainText.textInfo.meshInfo[materialIndex].mesh, materialIndex);

        }
        mainText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    /*
        Correctness values: 0 none, 1 correct, 2 incorrect
     */
    public void UpdateScreenText(string screenText, int[] correctnessValues, float battery, List<int> columnsToSwap = null)
    {
      creditsText.gameObject.SetActive(false);
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
        Vector3[] vertices = mainText.textInfo.meshInfo[materialIndex].vertices;
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
                if(i < correctnessValues.Length && correctnessValues[i] == 2)
                {
                    vertices[vertexIndex + j].y += wrongOffset;
                }
            }
        }
        mainText.textInfo.meshInfo[materialIndex].mesh.vertices = mainText.textInfo.meshInfo[materialIndex].vertices;
        mainText.UpdateGeometry(mainText.textInfo.meshInfo[materialIndex].mesh, materialIndex);
        mainText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    public void GameEnd()
    {
        GetComponent<Animator>().enabled = false;
        GetComponent<Flicker>().enabled = false;
        batteryText.gameObject.SetActive(false);
        mainText.gameObject.SetActive(false);
        endUI.SetActive(true);
        creditsText.gameObject.SetActive(true);
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
            if(lineEnd - (lineStart + columnToSwap + 1) >= 0 && lineStart + columnToSwap + 1 < lineEnd && lineEnd < mainText.text.Length)
            {
                temp = mainText.text[lineStart + columnToSwap];
                if(temp != '\n' && nextChar != '\n')
                {
                    if(lineStart + columnToSwap + 1 < lineEnd)
                        textByLine.Add(mainText.text.Substring(lineStart, columnToSwap) + nextChar
                            + mainText.text.Substring(lineStart + columnToSwap + 1, lineEnd - (lineStart + columnToSwap)));
                    else
                        textByLine.Add(mainText.text.Substring(lineStart, columnToSwap) + nextChar);
                    nextChar = temp;
                }
                else
                {
                    textByLine.Add(mainText.text.Substring(lineStart, lineEnd + 1 - lineStart));
                }
            }
            else
            {
                if(lineEnd < mainText.text.Length && lineEnd - lineStart > 0)
                    textByLine.Add(mainText.text.Substring(lineStart, lineEnd + 1 - lineStart));
            }
        }
        int firstlineStart = mainText.textInfo.lineInfo[0].firstCharacterIndex;
        int firstlineEnd = mainText.textInfo.lineInfo[0].lastCharacterIndex;
        if(firstlineEnd - (firstlineStart + columnToSwap + 1) > 0 && firstlineStart + columnToSwap + 1 < firstlineEnd)
        {
            if(nextChar != '\n')
            {
                if(firstlineStart + columnToSwap != firstlineEnd)
                    textByLine.Insert(0, mainText.text.Substring(firstlineStart, columnToSwap) + nextChar
                        + mainText.text.Substring(firstlineStart + columnToSwap + 1, firstlineEnd - (firstlineStart + columnToSwap)));
                else
                    textByLine.Insert(0, mainText.text.Substring(firstlineStart, columnToSwap) + nextChar);
            }
            else
            {
                if(firstlineEnd < mainText.text.Length && firstlineEnd - firstlineStart > 0)
                    textByLine.Insert(0, mainText.text.Substring(firstlineStart, firstlineEnd + 1 - firstlineStart));
            }
        }
        else
        {
            textByLine.Insert(0, mainText.text.Substring(firstlineStart, firstlineEnd + 1 - firstlineStart));
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
