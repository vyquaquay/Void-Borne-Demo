using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFading : MonoBehaviour
{

    [SerializeField] public float fadeTime;
    private Image fadeoutUIImage;
    public enum fadeDirection
    {
        In,
        Out
    }
    // Start is called before the first frame update
    void Awake()
    {
        fadeoutUIImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Fade(fadeDirection _fadeDirection)
    {
        float _alpha = _fadeDirection == fadeDirection.Out ? 1 : 0;
        float _fadeEndValue = _fadeDirection == fadeDirection.Out ? 0 : 1;

        if (_fadeDirection == fadeDirection.Out) 
        {
            while (_alpha >= _fadeEndValue) 
            {
                setColorImage(ref _alpha,_fadeDirection);
                yield return null;
            }
            fadeoutUIImage.enabled = false;
        }
        else
        {
            fadeoutUIImage.enabled = true;
            while(_alpha <= _fadeEndValue)
            {
                setColorImage(ref _alpha,_fadeDirection);
                yield return null;
            }
        }
        
    }

    public IEnumerator FadeAndLoadScene(fadeDirection _fadeDirection, string _sceneToLoad)
    {
        fadeoutUIImage.enabled = true ;
        yield return Fade(_fadeDirection);
        SceneManager.LoadScene(_sceneToLoad);
    }

    void setColorImage(ref float _alpha, fadeDirection _fadeDirection)
    {
        fadeoutUIImage.color = new Color(fadeoutUIImage.color.r, fadeoutUIImage.color.g, fadeoutUIImage.color.b, _alpha);
        _alpha += Time.deltaTime * (1/fadeTime) * (_fadeDirection == fadeDirection.Out ? -1 : 1);
    }

    public void CallFadeAndLoadScene(string _sceneToLoad)
    {
        StartCoroutine(FadeAndLoadScene(fadeDirection.In, _sceneToLoad));
    }
}
