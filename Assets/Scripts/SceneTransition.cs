using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private string transitionTo;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Vector2 directionExit;
    [SerializeField] private float exitTime;

    private void Start()
    {
        if (transitionTo == GameManager.Instance.transitionFromScene)
        {
            Move.Instance.transform.position = startPoint.position;
            StartCoroutine(Move.Instance.WalktonewScene(directionExit,exitTime));
        }

        StartCoroutine(UIManager.Instance.sceneFading.Fade(SceneFading.fadeDirection.Out));

    }
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            GameManager.Instance.transitionFromScene = SceneManager.GetActiveScene().name;
            Move.Instance.playerStateList.incutscene = true;
            Move.Instance.playerStateList.Invi = true;
            StartCoroutine(UIManager.Instance.sceneFading.FadeAndLoadScene(SceneFading.fadeDirection.In,transitionTo));
        }
    }
}
