using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkManagement : NetworkManager
{
    public override void OnStartServer()
    {
        Debug.Log($"Player spawn");
    }
    
    public override void OnStopServer()
    {
        Debug.Log($"Stop Server");
    }
    
    [Obsolete("Remove the NetworkConnection parameter in your override and use NetworkClient.connection instead.")]
    public override void OnClientConnect(NetworkConnection networkConnection)
    {
        Debug.Log($"Connect");
    }
    
    [Obsolete("Remove the NetworkConnection parameter in your override and use NetworkClient.connection instead.")]
    public override void OnClientDisconnect(NetworkConnection networkConnection)
    {
        Debug.Log($"Disconnect");
    }

}
