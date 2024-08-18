using System;
using System.Collections.Generic;
using DI;
using Manager;
using Mirror;
using UnityEngine;

namespace Net
{
    public class CustomNetworkManager : NetworkManager
    {
        public override void Awake()
        {
            base.Awake();
            
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
        }
    }
}
