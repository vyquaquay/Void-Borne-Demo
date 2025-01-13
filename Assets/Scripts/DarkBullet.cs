using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkBullet : MonoBehaviour
{

    [SerializeField] float Damaged;
    [SerializeField] float hitForce;
    [SerializeField] float Lifetime = 1;
    [SerializeField] int Speed;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, Lifetime);
    }

    private void FixedUpdate()
    {
        transform.position += Speed*transform.right;
    }
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.tag == "Enemy")
        {
            print("GetHit");
            _other.GetComponent<Enemy>().EnemyHit(Damaged,(_other.transform.position - transform.position).normalized, -hitForce);
        }
    }
}
