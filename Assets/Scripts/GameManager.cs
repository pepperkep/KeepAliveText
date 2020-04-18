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
    }

    // Update is called once per frame
    void Update()
    {
        screen.UpdateScreenText("The quick brown fox jumps\n over the lazy dog", new int[] {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1
        ,1,1,1,1,1,1,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0});
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
