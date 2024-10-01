namespace TowerDefenceGame.Server.Entities
{
    public class Map
    {
        public int[,] Grid { get; set; }

        public Map(int rows, int cols)
        {
            Grid = new int[rows, cols];
        }

        public bool PlaceTower(int x, int y)
        {
            if (Grid[x, y] == 0)
            {
                Grid[x, y] = 1; 
                return true;
            }
            return false;
        }

    }
}
