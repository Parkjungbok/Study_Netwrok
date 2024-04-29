using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] float moveSpeed;

    public Vector3 velocity { get { return rigid.velocity; } }

    private void Awake()
    {
        rigid.velocity = transform.forward * moveSpeed;
        Destroy(gameObject, 5f);
    }    
}
