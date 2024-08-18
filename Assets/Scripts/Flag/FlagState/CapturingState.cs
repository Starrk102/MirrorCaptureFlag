using Net;
using UnityEngine;

namespace Flag.FlagState
{
    public class CapturingState : State
    {
        private Net.ClientRpc clientRpc;
        
        public CapturingState(Flag flag) : base(flag)
        {
            clientRpc = Installer.Installer.Container.Resolve<ClientRpc>();
        }

        public override void Enter()
        {
            flag.remainingCaptureTime = UnityEngine.Random.Range(flag.minCaptureTime, flag.maxCaptureTime);
            if (UnityEngine.Random.value < 0.5f)
            {
                flag.miniGame.StartMiniGame(flag, flag.remainingCaptureTime);
            }
        }

        public override void Update()
        {
            if (!flag.IsPlayerInCaptureRadius())
            {
                flag.TransitionToState(new IdleState(flag));
                flag.miniGame.EndMiniGame();
                clientRpc.SendMessage(flag.isServer, flag.player.uiManager, "Флаг команды " + flag.colorName + " не был захвачен");
                return;
            }

            if (flag.miniGame.isPlaying)
            {
                // Ждем, пока мини-игра не закончится
                return;
            }

            flag.remainingCaptureTime -= Time.deltaTime;

            if (flag.remainingCaptureTime <= 0)
            {
                flag.TransitionToState(new CapturedState(flag));
            }
        }
    }
}