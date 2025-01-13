using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feetcheck : MonoBehaviour
{
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private Transform playerFeet;
    [SerializeField] private LayerMask layerMask;
    private RaycastHit2D hit2D;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Groundcheck();
    }
    private void Groundcheck()
    {
        hit2D = Physics2D.Raycast(raycastOrigin.position, -Vector2.up, 100f, layerMask);
        if (hit2D != false)
        {
            Vector2 temp = playerFeet.position;
            temp.y = hit2D.point.y;
            playerFeet.position = temp;
        }
    }
}
