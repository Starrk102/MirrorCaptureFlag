namespace Flag.FlagState
{
    public class IdleState : State
    {
        public IdleState(Flag flag) : base(flag)
        {
        }

        public override void Enter()
        {
            flag.remainingCaptureTime = UnityEngine.Random.Range(flag.minCaptureTime, flag.maxCaptureTime);
        }

        public override void Update()
        {
            if (flag.IsPlayerInCaptureRadius())
            {
                flag.TransitionToState(new CapturingState(flag));
            }
            /*else
            {
                clientRpc.RpcCaptureFlag(flag.gameObject.GetComponent<MeshRenderer>().materials[0].color);
                flag.TransitionToState(new IdleState(flag));
            }*/
        }
    }
}