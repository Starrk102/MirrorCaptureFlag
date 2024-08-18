namespace Flag.FlagState
{
    public abstract class State
    {
        protected Flag flag;

        protected State(Flag flag)
        {
            this.flag = flag;
        }

        public abstract void Enter();
        public abstract void Update();
    }
}