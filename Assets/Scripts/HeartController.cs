using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HeartController : MonoBehaviour
{
    Move Player;
    private GameObject[] heartContainer;
    private Image[] heartFill;
    public Transform heartParent;
    public GameObject heartContainerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Player = Move.Instance;
        heartContainer = new GameObject[Move.Instance.maxTotalHealth];
        heartFill = new Image[Move.Instance.maxTotalHealth];

        Move.Instance.onHealthChangedCallback += updateHeartHUD;
        InstantiateHeartContainer();
        updateHeartHUD();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SetHeartContainer()
    {
        for (int i = 0; i < heartContainer.Length; i++)
        {
            if (i < Move.Instance.MaxHealth)
            {
                heartContainer[i].SetActive(true);
            }
            else 
            {
                heartContainer[i].SetActive(false);
            }
        }
    }
    void SetFilledHeart()
    {
        for (int i = 0; i < heartFill.Length; i++)
        {
            if (i < Move.Instance.Health)
            {
                heartFill[i].fillAmount = 1;
            }
            else
            {
                heartFill[i].fillAmount = 0;
            }
        }
    }

    void InstantiateHeartContainer()
    {
        if (heartContainerPrefab == null || heartParent == null)
        {
            Debug.LogError("Heart Container Prefab or Heart Parent is not assigned!");
            return;
        }

        // Clear existing hearts (if any)
        foreach (Transform child in heartParent)
        {
            Destroy(child.gameObject);
        }

        // Instantiate hearts based on maxTotalHealth
        for (int i = 0; i < Move.Instance.maxTotalHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab, heartParent);
            heartContainer[i] = temp;

            // Ensure heartFill object is correctly found
            Image fillImage = temp.transform.Find("heartfill")?.GetComponent<Image>();
            if (fillImage != null)
            {
                heartFill[i] = fillImage;
            }
            else
            {
                Debug.LogError($"Heart {i} - 'heartfill' Image component not found in prefab.");
            }

            // Log each heart creation to debug if it's being skipped
            Debug.Log($"Heart {i + 1}/{Move.Instance.maxTotalHealth} created.");
        }
    }

    void updateHeartHUD()
    {
        SetHeartContainer();
        SetFilledHeart();
    }
}
