using System;
using DI;
using Manager;
using Mirror;
using Net;
using UnityEngine;

namespace Installer
{
    public class Installer : MonoBehaviour
    {
        [SerializeField] private CustomNetworkManager customNetworkManager;
        [SerializeField] private GameManager gameManager;
        
        private static DIContainer container;
        public static DIContainer Container => container;
        
        private void Awake()
        {
            container = new DIContainer();

            CreateClientRpc();
            CreateGameManager();
            CreateNetworkManager();
        }

        private void CreateNetworkManager()
        {
            var instance = Instantiate(customNetworkManager, new RectTransform());
            container.RegisterInstance<CustomNetworkManager>(instance);
        }
        
        private void CreateGameManager()
        {
            var instance = Instantiate(gameManager, new RectTransform());
            container.RegisterInstance<GameManager>(instance);
        }

        private void CreateClientRpc()
        {
            var clientRpc = new ClientRpc();
            container.RegisterInstance<ClientRpc>(clientRpc);
        }
    }
}