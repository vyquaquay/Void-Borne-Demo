using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuFadeController : MonoBehaviour
{
    private fadedUI fadedUI;
    [SerializeField] private float fadeTime;
    // Start is called before the first frame update
    void Start()
    {
        fadedUI = GetComponent<fadedUI>();
        fadedUI.fadeUIOut(fadeTime);
    }

    public void callFadeandLoadGame(string _sceneToLoad)
    {
        StartCoroutine(fadeandStartGame(_sceneToLoad));
    }

    IEnumerator fadeandStartGame(string _sceneToLoad)
    {
        fadedUI.fadeUIIn(fadeTime);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene( _sceneToLoad ); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
