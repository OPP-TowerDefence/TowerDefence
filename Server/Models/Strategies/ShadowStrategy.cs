using System.Collections.Generic;
using System.Linq;
using TowerDefense.Interfaces;
using TowerDefense.Models;
using TowerDefense.Models.Enemies;

namespace TowerDefense.Models.Strategies
{
    public class ShadowStrategy : IPathStrategy
    {
        public StrongEnemy targetStrongEnemy = null;
        private Queue<PathPoint> delayedPath = new Queue<PathPoint>();
        private int delayTiles = 1;
        private int waitDuration = 3;
        public bool HasTarget() => targetStrongEnemy != null;

        public List<PathPoint> SelectPath(GameState gameState, Enemy enemy)
        {
            return delayedPath.Count > 0 ? delayedPath.ToList() : new List<PathPoint>();
        }
        public void SetTargetStrongEnemy(StrongEnemy strongEnemy)
        {
            if (targetStrongEnemy == null)
            {
                targetStrongEnemy = strongEnemy;
                delayedPath.Clear();

                if (strongEnemy.Path != null)
                {
                    var fullPath = strongEnemy.Path.ToList();
                    delayedPath = ApplyDelay(fullPath, strongEnemy);
                }
            }
        }
        private Queue<PathPoint> ApplyDelay(List<PathPoint> fullPath, Enemy enemy)
        {
            var delayedQueue = new Queue<PathPoint>();
            var currentTile = new PathPoint(enemy.X, enemy.Y, TileType.Normal);

            for (int w = 0; w < waitDuration; w++)
            {
                delayedQueue.Enqueue(currentTile);
            }

            for (int i = delayTiles; i < fullPath.Count; i++)
            {
                delayedQueue.Enqueue(fullPath[i]);
            }
            return delayedQueue;
        }
        public void StopFollowing()
        {
            targetStrongEnemy = null;
        }
    }
}
