using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PhotonView view;
    [SerializeField] PlayerInput input;
    [SerializeField] float moveSpeed;

    private Vector3 moveDir;

    private void Awake()
    {
        if (view.IsMine == false)
        {
            Destroy(input);
        }
    }

    private void Update()
    {        
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }
    private void OnMove(InputValue value)
    {
        moveDir.x = value.Get<Vector2>().x;
        moveDir.z = value.Get<Vector2>().y;
    }
}
