using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Mirror;
public class Controller : NetworkBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }
    
    void HandleMovement()
    {
        if (!isLocalPlayer) return;
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal * 0.1f, moveVertical * 0.1f, 0);
        this.transform.position +=  movement;
    }
    
    public override void OnStartLocalPlayer()
    {
        var setCamera = GameObject.FindWithTag("Cam").GetComponent<CinemachineVirtualCamera>();
        var gameobject = this.gameObject;
        setCamera.LookAt = gameobject.transform;
        setCamera.Follow = gameobject.transform;
    }
}
