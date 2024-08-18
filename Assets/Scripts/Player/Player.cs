using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Manager;
using Mirror;
using Net;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class Player : NetworkBehaviour
    {
        public RectTransform joystickBackground;
        public RectTransform joystickHandle;
        public Camera camera;
        public float moveSpeed = 10f;
        public float handleRange = 100f;
        public float rotateSpeed = 100f;
        
        public ColorEnum party;
        public UIManager uiManager;
        public MiniGame miniGame;
        
        [SyncVar]public List<Flag.Flag> flags = new List<Flag.Flag>(3);
        
        private Vector2 inputDirection;
        private GameManager gameManager;
        private ClientRpc clientRpc;
        
        private void Start()
        {
            if (!isLocalPlayer)
            {
                camera.enabled = false;
                camera.gameObject.GetComponent<AudioListener>().enabled = false;
            }
        }

        private void Update()
        {
            if(!isLocalPlayer)
                return;
            HandleJoystick();
            MovePlayer();
        }

        public override void OnStartClient()
        {
            gameManager = Installer.Installer.Container.Resolve<GameManager>();
            clientRpc = Installer.Installer.Container.Resolve<ClientRpc>();
            
            base.OnStartClient();
            
            gameManager.AddPlayerCollections(this);
            //CreateFlag(this);
        }

        public void ColorPlayers(ColorEnum party)
        {
            if (!isServer)
                SetMaterialAndParty(party);
            else
                SetMaterialAndPartyRpc(party);
        }
        
        [Command(requiresAuthority = false)]
        private void SetMaterialAndParty(ColorEnum party)
        {
            SetMaterialAndPartyRpc(party);
        }

        [ClientRpc]
        private void SetMaterialAndPartyRpc(ColorEnum party)
        {
            this.GetComponent<MeshRenderer>().material = gameManager.materialColor[(int)party];
            this.party = party;
        }

        private void HandleJoystick()
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 touchPosition = Input.mousePosition;

                Vector2 joystickPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, touchPosition, null,
                    out joystickPosition);

                Vector2 offset = joystickPosition / joystickBackground.sizeDelta;
                inputDirection = new Vector2(Mathf.Clamp(offset.x, -1f, 1f), Mathf.Clamp(offset.y, -1f, 1f));

                joystickHandle.anchoredPosition = inputDirection * handleRange;
            }
            else
            {
                joystickHandle.anchoredPosition = Vector2.zero;
                inputDirection = Vector2.zero;
            }
        }

        private void MovePlayer()
        {
            Vector3 moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
            Vector3 localMoveDirection = transform.TransformDirection(moveDirection);
            transform.Translate(localMoveDirection * (moveSpeed * Time.deltaTime), Space.World);

            float rotation = inputDirection.x * rotateSpeed * Time.deltaTime;
            transform.Rotate(0, rotation, 0);
        }

        public void CreateFlag()
        {
            if (isServer)
            {
                CreateSpawnableObjects();
            }
        }
        
        private void CreateSpawnableObjects()
        {
            for (int i = 0; i < 3; i++)
            {
                var flagObject = Instantiate(gameManager.flag, new Vector3(Random.Range(-50, 50), 1, Random.Range(-50, 50)), Quaternion.identity);
                
                var flagComponent = flagObject.GetComponent<Flag.Flag>();
                flagComponent.player = this;
                
                NetworkServer.Spawn(flagObject);
            }
        }

        public void AddFlag(Flag.Flag flag)
        {
            //if (flag.colorName == this.party)
            //{
                flags.Add(flag);
            //}
        }
        
        public void FlagCaptured(ColorEnum teamColor)
        {
            CheckForWin(teamColor);
        }
        
        private void CheckForWin(ColorEnum teamColor)
        {
            Debug.Log(CheckDisableElementCollections());
            if (CheckDisableElementCollections())
            {
                EndGame(teamColor);
            }
        }

        private bool CheckDisableElementCollections()
        {
            return flags.All(flag => flag.isCaptured);
        }
        
        [Server]
        private void EndGame(ColorEnum winningTeam)
        {
            clientRpc.SendMessage(isServer, uiManager, $"Команда {winningTeam} выиграла!");
            //Debug.Log($"Команда {winningTeam} выиграла!");
            NetworkServer.DisconnectAll();
        }
    }
}