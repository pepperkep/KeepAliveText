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
    public void UpdateScreenText(string screenText, int[] correctnessValues, float battery)
    {
        mainText.text = screenText;
        mainText.ForceMeshUpdate();
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

            for(int j = 0; j < 4; j++)
            {
                nextColors[vertexIndex + j] = nextColor;
            }
        }
        mainText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private Color ChangeBrightness(Color originalColor, float brightnessLevel)
    {
        float H, S, V;
        Color.RGBToHSV(originalColor, out H, out S, out V);
        return Color.HSVToRGB(H, S, V * brightnessLevel / 100);
    }
}
