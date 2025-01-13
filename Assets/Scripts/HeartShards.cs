using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartShards : MonoBehaviour
{

    public Image fill;
    public float targetFillAmount;
    public float lerpDuration= 1.5f;
    public float initalFillAmount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator LerpFill()
    {
        float elapseTime = 0f;
        while(elapseTime < lerpDuration)
        {
            elapseTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapseTime / lerpDuration);
            float lerpFillAmount = Mathf.Lerp(initalFillAmount, targetFillAmount, t);
            fill.fillAmount = lerpFillAmount;
            yield return null;
        }

        fill.fillAmount = targetFillAmount;
        if (fill.fillAmount == 1)
        {
            Move.Instance.MaxHealth++;
            Move.Instance.onHealthChangedCallback();
            Move.Instance.heartShards = 0;
            SaveData.Instance.savePlayerData(); // Save immediately after increasing MaxHealth
        }

    }
}
