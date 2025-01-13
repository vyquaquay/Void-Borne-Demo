using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadedUI : MonoBehaviour
{

    CanvasGroup CanvasGroup;

    private void Awake()
    {
        CanvasGroup = GetComponent<CanvasGroup>();
    }


    public void fadeUIOut(float _seconds)
    {
        StartCoroutine(fadeOut(_seconds));
    }
    public void fadeUIIn(float _seconds)
    {
        StartCoroutine(fadeIn(_seconds));
    }
    IEnumerator fadeOut(float _seconds)
    {
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
        CanvasGroup.alpha = 1;
        while(CanvasGroup.alpha > 0)
        {
            CanvasGroup.alpha -= Time.unscaledDeltaTime / _seconds;
            yield return null;
        }
        yield return null;
    }
    IEnumerator fadeIn(float _seconds)
    {
        
        CanvasGroup.alpha = 0;
        while (CanvasGroup.alpha <  1)
        {
            CanvasGroup.alpha += Time.unscaledDeltaTime / _seconds;
            yield return null;
        }
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;
        yield return null;
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
