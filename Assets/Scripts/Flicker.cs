using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    [SerializeField] private AnimationCurve flicker1Prob;
    [SerializeField] private AnimationCurve flicker2Prob;
    [SerializeField] private AnimationCurve flicker3Prob;
    [SerializeField] private float checkFlickerTime = 5f;
    private Animator animComponent;
    private IEnumerator flickerRoutine;

    // Start is called before the first frame update
    void Start()
    {
        animComponent = GetComponent<Animator>();
        flickerRoutine = CheckFlicker();
        StartCoroutine(flickerRoutine);
    }

    // Update is called once per frame
    private IEnumerator CheckFlicker()
    {
        float currentPower = 100f;
        while(currentPower > 0)
        {
            currentPower = GameManager.Instance().Power;
            if(Random.value < flicker1Prob.Evaluate(currentPower))
            {
                animComponent.SetBool("FirstFlicker", true);
            }
            if(Random.value < flicker2Prob.Evaluate(currentPower))
            {
                animComponent.SetBool("SecondFlicker", true);
            }
            if(Random.value < flicker3Prob.Evaluate(currentPower))
            {
                animComponent.SetBool("ThirdFlicker", true);
            }
            yield return new WaitForSeconds(checkFlickerTime);
        }
    }
}
