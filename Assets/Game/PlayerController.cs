using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] List<Color> colorList;

    [SerializeField] PlayerInput input;

    [SerializeField] Bullet bulletPrefab;
    [SerializeField] float coolTime;

    [SerializeField] float lastFireTime;

    [SerializeField] float movePower;
    [SerializeField] float maxSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] int fireCount;
    [SerializeField] int testValue;
    [SerializeField] float currentSpeed;

    private Vector3 moveDir;

    private void Awake()
    {
        if (photonView.IsMine == false)
        {
            Destroy(input);
        }

        SetPlayerColor();
    }

    private void Update()
    {
        Accelate();
        Rotate();
    }

    private void Accelate()
    {
        rigid.AddForce(moveDir.z * transform.forward * movePower, ForceMode.Force);
        if (rigid.velocity.magnitude > maxSpeed)
        {
            rigid.velocity = rigid.velocity.normalized * maxSpeed;
        }

        currentSpeed = rigid.velocity.magnitude;
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up, moveDir.x * rotateSpeed * Time.deltaTime);
    }

    private void OnMove(InputValue value)
    {
        moveDir.x = value.Get<Vector2>().x;
        moveDir.z = value.Get<Vector2>().y;
    }

    private void OnFire(InputValue value)
    {
        photonView.RPC("CreateBullet", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void RequestCreateBullet()
    {
        if (Time.time < lastFireTime + coolTime) return;

        lastFireTime = Time.time;
        photonView.RPC("ResultCreateBullet", RpcTarget.AllViaServer, transform.position, transform.rotation);
    }

    [PunRPC] //원격 함수 호출
    private void ResultCreateBullet(Vector3 position, Quaternion rotation, PhotonMessageInfo info)
    {
        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

        fireCount++;
        Bullet bullet = Instantiate(bulletPrefab, position, rotation);
        bullet.transform.position += bullet.Velocity * lag;
    }


    private void SetPlayerColor()
    {
        int playerNumber = photonView.Owner.GetPlayerNumber();
        if (colorList == null || colorList.Count <= playerNumber) return;

        Renderer render = GetComponent<Renderer>();
        render.material.color = colorList[playerNumber];

        if (photonView.IsMine)
        {
            render.material.color = Color.green;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 다른 플레이어의 오브젝트를 참조하는법
        GameObject rigid = PhotonView.Find(photonView.ViewID).gameObject;

        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(fireCount);
            stream.SendNext(testValue);
            stream.SendNext(currentSpeed);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            testValue = (int)stream.ReceiveNext();
            fireCount = (int)stream.ReceiveNext();
            currentSpeed = (float)stream.ReceiveNext();
        }
    }
}
