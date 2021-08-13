namespace Hipparu.Pages
{
    public partial class Index
    {
        private int gameMode = 0;

        void IncrementCount(int mode)
        {
            if (mode == 0)
            {
                gameMode++;
            }
            else
            {
                gameMode = 0;
            }
        }
    }
}