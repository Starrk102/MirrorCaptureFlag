using System;
using Mirror;
using UnityEngine;

namespace Net
{
    public class AutoNetworkManager : MonoBehaviour
    {
        public string hostIP = "127.0.0.1";
        public int maxPlayers = 3;

        private void Start()
        {
            NetworkManager.singleton.networkAddress = hostIP;

            if (!NetworkServer.active)
            {
                StartHost();
            }
            else
            {
                StartClient();
            }
        }

        private void StartClient()
        {
            NetworkManager.singleton.StartClient();
        }

        private void StartHost()
        {
            NetworkManager.singleton.maxConnections = maxPlayers;
            NetworkManager.singleton.StartHost();
            Debug.Log("Запуск сервера...");
        }
    }
}
