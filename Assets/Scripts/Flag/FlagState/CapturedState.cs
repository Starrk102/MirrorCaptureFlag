using Manager;
using Net;
using UnityEngine;

namespace Flag.FlagState
{
    public class CapturedState : State
    {
        private GameManager gameManager;
        private Net.ClientRpc clientRpc;
        
        public CapturedState(Flag flag) : base(flag)
        {
            gameManager = Installer.Installer.Container.Resolve<GameManager>();
            clientRpc = Installer.Installer.Container.Resolve<ClientRpc>();
        }

        public override void Enter()
        {
            flag.player.FlagCaptured(flag.colorName);
            flag.CapturedFlag();
            clientRpc.SendMessage(flag.isServer, flag.player.uiManager, "Флаг команды " + flag.colorName + " был захвачен");
        }

        public override void Update()
        {
            
        }
    }
}