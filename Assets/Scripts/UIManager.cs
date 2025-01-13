using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static UIManager Instance;

    [SerializeField] GameObject DeathSceen;
    public GameObject mapHandler;
    public GameObject inventory;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject.transform.root.gameObject);
    }

    public SceneFading sceneFading;
    public void Start()
    {
        sceneFading = GetComponentInChildren<SceneFading>();
    }

    public IEnumerator ActiveDeathScreen()
    {
        yield return new WaitForSeconds(0.8f);
        StartCoroutine(sceneFading.Fade(SceneFading.fadeDirection.In));
        yield return new WaitForSeconds(0.9f);
        DeathSceen.SetActive(true);
    }

    public IEnumerator DeactiveDeathScreen()
    {
        Debug.Log("Deactivating Death Screen…");
        yield return new WaitForSeconds(0.25f);
        DeathSceen.SetActive(false);
        StartCoroutine(sceneFading.Fade(SceneFading.fadeDirection.Out));
    }
}
