using System;
using System.Collections;
using System.Threading.Tasks;
using DI;
using Enums;
using Flag.FlagState;
using Manager;
using Mirror;
using Net;
using NUnit.Framework.Constraints;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Flag
{
    public class Flag : NetworkBehaviour
    {
        public float captureRadius = 5f;
        public float minCaptureTime = 3f;
        public float maxCaptureTime = 6f;
        [SyncVar]public ColorEnum colorName;
        public GameObject radiusObject;
        [SyncVar]public Player.Player player;

        [SyncVar]public bool isCaptured = false;
        private GameManager gameManager;
        private Net.ClientRpc clientRpc;
        private State currentState;
        public float remainingCaptureTime;
        public MiniGame miniGame => player.miniGame;
            
        private void Start()
        {
            gameManager = Installer.Installer.Container.Resolve<GameManager>();
            clientRpc = Installer.Installer.Container.Resolve<ClientRpc>();

            GetComponent<MeshRenderer>().material = gameManager.materialColor[(int)player.party];
            colorName = player.party;
            radiusObject.GetComponent<MeshRenderer>().material =
                this.gameObject.GetComponent<MeshRenderer>().material;

            player.AddFlag(this);
            
            TransitionToState(new IdleState(this));
        }

        private void Update()
        {
            currentState.Update();
            //Debug.Log(currentState);
        }

        public bool IsPlayerInCaptureRadius()
        {
             return Vector3.Distance(transform.position, player.gameObject.transform.position) <= captureRadius;
        }
        
        public void ResetCapture()
        {
            TransitionToState(new IdleState(this));
            clientRpc.SendMessage(isServer, player.uiManager, "Флаг команды " + colorName + " не был захвачен");
        }

        public void TransitionToState(State newState)
        {
            currentState = newState;
            currentState.Enter();
        }
        
        public void CapturedFlag()
        {
            if (!isServer)
                CapturedFlagCmd();
            else
                CapturedFlagRpc();
        }
        
        [Command(requiresAuthority = false)]
        private void CapturedFlagCmd()
        {
            CapturedFlagRpc();
        }

        [ClientRpc]
        private void CapturedFlagRpc()
        {
            if (!isCaptured)
            {
                isCaptured = true;
                player.FlagCaptured(player.party);
                gameObject.SetActive(false);
            }
        }
    }
}
