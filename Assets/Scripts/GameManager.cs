using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float power = 100f;
    [SerializeField] private float drainAmount = 0.05f;
    [SerializeField] private float drainInterval = 1f;
    [SerializeField] private ScreenManager screen;
    private IEnumerator drainRoutine;
    private string currentTextInput;
    private string correctText = "The quick brown fox jumps\nover the lazy dog";
    private List<int> correctnessList = new List<int>();
    private int correctIndex;

    public float Power
    {
        get => power;
        set => power = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        drainRoutine = DrainBattery(drainAmount, drainInterval);
        StartCoroutine(drainRoutine);
        currentTextInput = "";
        correctIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (char c in Input.inputString)
        {
            if (c == '\b') // has backspace/delete been pressed?
            {
                if (currentTextInput.Length != 0)
                {
                    currentTextInput = currentTextInput.Substring(0, currentTextInput.Length - 1);
                    correctnessList.RemoveAt(correctnessList.Count - 1);
                    correctIndex--;
                }
            }
            else if ((c == '\n') || (c == '\r')) // enter/return
            {
                currentTextInput = "";
                correctnessList = new List<int>();
                correctIndex = 0;
            }
            else
            {
                currentTextInput += c;
                if(correctIndex < correctText.Length && c == correctText[correctIndex])
                    correctnessList.Add(1);
                else
                    correctnessList.Add(2);
                correctIndex++;
            }
        }
        screen.UpdateScreenText(correctText, correctnessList.ToArray());
    }

    private IEnumerator DrainBattery(float amount, float interval)
    {
        while(power > 0)
        {
            yield return new WaitForSeconds(interval);
            Power -= amount;
            screen.SetBatteryText(power);
        }
    }
}
