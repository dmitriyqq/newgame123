namespace GameModel
{
    public interface IGameLoop
    {
        Toggle IsPlaying { get; set; }

        void Exit();
    }
}