using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    [SerializeField] GameObject[] maps;
    Alter alter;

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame


    public void UpdateMap()
    {
        var savedScene = SaveData.Instance.sceneNames;
        for (int i = 0; i < maps.Length; i++)
        {
            if(savedScene.Contains("Map_"+(i+1)))
            {
                maps[i].SetActive(true);
            }
            else
            {
                maps[i].SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        alter = FindObjectOfType<Alter>();
        if(alter != null)
        {
            if (alter.Interacted)
            {
                UpdateMap();
            }
        }
    }
}
