using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI batteryText;
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private Color32 defaultColor;
    [SerializeField] private Color32 correctColor;
    [SerializeField] private Color32 wrongColor;

    public void SetBatteryText(float batteryValue)
    {
        float roundedValue = Mathf.Round(batteryValue * 10) / 10;
        string nextBatteryText = "" + roundedValue;
        if(roundedValue % 1 == 0)
            nextBatteryText += ".0";
        nextBatteryText += "%";
        batteryText.text = nextBatteryText;
    }

    /*
        Correctness values: 0 none, 1 correct, 2 incorrect
     */
    public void UpdateScreenText(string screenText, int[] correctnessValues)
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
                        nextColor = correctColor;
                        break;
                    case 2:
                        nextColor = wrongColor;
                        break;
                    default:
                        nextColor = defaultColor;
                        break;
                }
            }
            else
            {
                nextColor = defaultColor;
            }

            for(int j = 0; j < 4; j++)
            {
                nextColors[vertexIndex + j] = nextColor;
            }
        }
        mainText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
