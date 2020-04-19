﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float power = 100f;
    [SerializeField] private AnimationCurve drainAmount;
    [SerializeField] private float drainInterval = 1f;
    [SerializeField] private float correctnessToEnergyConversion = 0.2f;
    [SerializeField] private ScreenManager screen;
    [SerializeField] private List<char> glitchingCharacters = new List<char>();
    [SerializeField] private int minGlitchAmount = 1;
    [SerializeField] private int maxGlitchAmount = 3;
    [SerializeField] private List<int> glitchLevels;
    private int textsIndex = 0;
    private IEnumerator drainRoutine;
    private string currentTextInput;
    private List<string> texts = new List<string>();
    private string correctText = "Please send help ASAP. There's a man who broke in to the house and he's looking for cookies.";
    private string glitchedText;
    private int glitchLevel;
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
        LoadText lt=gameObject.GetComponent(typeof(LoadText)) as LoadText;
        textsIndex = 0;
        texts=lt.GetMyTexts();
        currentTextInput = "";
        correctIndex = 0;
        glitchedText = correctText;
        glitchLevel = 0;
        screen.UpdateScreenText(correctText, correctnessList.ToArray(), Power);
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
                    screen.UpdateCharacterColor(correctnessList.Count - 1, 0, Power);
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
                glitchLevel = 0;
                textsIndex = (textsIndex + 1) % texts.Count;
                correctText = texts[textsIndex];
                glitchedText = correctText;
                screen.UpdateScreenText(correctText, correctnessList.ToArray(), Power);
            }
            else
            {
                currentTextInput += c;
                if(correctIndex < correctText.Length && (c == correctText[correctIndex] ||
                    (c == ' ' && correctText[correctIndex] == '\n')))
                {
                    screen.UpdateCharacterColor(correctnessList.Count, 1, Power);
                    correctnessList.Add(1);
                }
                else
                {
                    screen.UpdateCharacterColor(correctnessList.Count, 2, Power);
                    correctnessList.Add(2);
                }
                correctIndex++;
            }
        }
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
        screen.SetBatteryText(power);
        screen.UpdateScreenText(correctText, correctnessList.ToArray(), Power);
    }

    private string ReplaceTextSymbols(string originalText, int glitchAmount)
    {
        char[] delimiterChars = {' ', '\n', '\t' };
        string[] words = originalText.Split(delimiterChars);
        for(int i = 0; i < glitchAmount; i++)
        {
            int numGlitches = Random.Range(minGlitchAmount, maxGlitchAmount);
            for(int j = 0 ; j < numGlitches; j++)
            {
                int glitchIndex = Random.Range(0, words.Length);
                string glitchWord = words[glitchIndex];
                int letterIndex = Random.Range(0, glitchWord.Length);
                if(letterIndex != glitchWord.Length)
                    words[glitchIndex] = glitchWord.Substring(0, letterIndex) + glitchingCharacters[Random.Range(0, glitchingCharacters.Count)]
                                                + glitchWord.Substring(letterIndex + 1);
                else
                    words[glitchIndex] = glitchWord.Substring(0, letterIndex) + glitchingCharacters[Random.Range(0, glitchingCharacters.Count)];
            }
        }
        string finalText = "";
        for(int i = 0; i < words.Length - 1; i++)
        {
            finalText += words[i] + " ";
        }
        return finalText + words[words.Length - 1];
    }

    private IEnumerator DrainBattery(AnimationCurve amount, float interval)
    {
        float totalTime = 0;
        while(Power > 0)
        {
            yield return new WaitForSeconds(interval);
            totalTime += interval;
            Power -= amount.Evaluate(totalTime);
            int currentGlitch = glitchLevels[(int)(Power / 10)];
            glitchedText = ReplaceTextSymbols(glitchedText, currentGlitch - glitchLevel);
            glitchLevel = currentGlitch;
            screen.SetBatteryText(power);
            screen.UpdateScreenText(glitchedText, correctnessList.ToArray(), Power);
        }
    }
}
