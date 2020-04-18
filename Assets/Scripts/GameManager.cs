using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float power = 100f;
    [SerializeField] private AnimationCurve drainAmount;
    [SerializeField] private float drainInterval = 1f;
    [SerializeField] private float correctnessToEnergyConversion = 0.2f;
    [SerializeField] private ScreenManager screen;
    private IEnumerator drainRoutine;
    private string currentTextInput;
    private List<string> texts = new List<string>();
    private int textsIndex = 0;
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
        texts.Add("The quick brown fox jumps\nover the lazy dog");
        texts.Add("Lorem ipsum dolor sit amet,\nconsectetur adipiscing elit, sed do eiusmod tempor\nincididunt ut labore et dolore magna aliqua.");
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
                AddBattery(correctnessList);
                currentTextInput = "";
                correctnessList = new List<int>();
                correctIndex = 0;
                textsIndex = (textsIndex + 1) % texts.Count;
                correctText = texts[textsIndex];    
            }
            else
            {
                currentTextInput += c;
                if(correctIndex < correctText.Length && (c == correctText[correctIndex] ||
                    (c == ' ' && correctText[correctIndex] == '\n')))
                    correctnessList.Add(1);
                else
                    correctnessList.Add(2);
                correctIndex++;
            }
        }
        screen.UpdateScreenText(correctText, correctnessList.ToArray());
    }

    private void AddBattery(List<int> correctness)
    {
        float addedEnergy = 0;
        for(int i = 0; i < correctness.Count; i++)
        {
            addedEnergy += correctness[i];
        }
        addedEnergy /= correctnessToEnergyConversion;
        if(Power + addedEnergy > 100)
            Power = 100;
        else
            Power += addedEnergy;
    }

    private IEnumerator DrainBattery(AnimationCurve amount, float interval)
    {
        float totalTime = 0;
        while(Power > 0)
        {
            yield return new WaitForSeconds(interval);
            totalTime += interval;
            Power -= amount.Evaluate(totalTime);
            screen.SetBatteryText(power);
        }
    }
}
