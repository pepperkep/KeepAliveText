using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenManager : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI batteryText;

    public void SetBatteryText(float batteryValue)
    {
        float roundedValue = Mathf.Round(batteryValue * 10) / 10;
        string nextBatteryText = "" + roundedValue;
        if(roundedValue % 1 == 0)
            nextBatteryText += ".0";
        nextBatteryText += "%";
        batteryText.text = nextBatteryText;
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
