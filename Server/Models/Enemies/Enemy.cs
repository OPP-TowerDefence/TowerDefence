using TowerDefense.Enums;
using TowerDefense.Models.Towers;
using TowerDefense.Utils;
using TowerDefense.Models.Strategies;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Enemies
{
    public abstract class Enemy : Unit, IEnemyComponent
    {
        public Guid Id { get; set; }
        public int Health { get; set; }
        public int RewardValue { get; set; } = 10;
        public int Speed { get; set; }
        public Queue<PathPoint> Path { get; set; }
        protected int _currentSpeedModifier = 0;
        protected int _modifierDuration = 0;
        protected (int x, int y) _lastTilePosition;
        private bool IsAtFinalDestination { get; set; } = false;
        public IPathStrategy CurrentStrategy { get; private set; }
        public bool IsShadowEnemy { get; private set; } = false;
        public abstract EnemyTypes Type { get; }
        const int lowHealthThreshold = 20;
        const int closeDistanceThreshold = 15;
        public List<(ITileEffect effect, int turnsRemaining)> _scheduledEffects = new List<(ITileEffect, int)>();

        public Enemy(int x, int y) : base(x, y)
        {
            Id = Guid.NewGuid();
            _lastTilePosition = (x, y);
        }

        public void SetInitialStrategy(IPathStrategy strategy)
        {
            CurrentStrategy = strategy;
        }

        public void RetrievePath(GameState gameState)
        {
            if (CurrentStrategy != null)
            {
                List<PathPoint> selectedPath = CurrentStrategy.SelectPath(gameState, this);
                Path = new Queue<PathPoint>(selectedPath);
            }
        }

        private void SetStrategy(IPathStrategy strategy)
        {
            CurrentStrategy = strategy;
        }

        public void ClearSpeedModifier()
        {
            _currentSpeedModifier = 0;
            _modifierDuration = 0;
        }

        public void ApplySpeedModifier(int modifier, int duration)
        {
            if (_modifierDuration <= 0)
            {
                _currentSpeedModifier += modifier;
                _modifierDuration = duration;
            }
        }

        public void ScheduleEffect(ITileEffect effect, int delay)
        {
            if (delay <= 0)
            {
                effect.ApplyEffect(this);
            }
            else
            {
                _scheduledEffects.Add((effect, delay));
            }
        }

        public void UpdateScheduledEffects()
        {
            for (int i = _scheduledEffects.Count - 1; i >= 0; i--)
            {
                var (effect, turnsRemaining) = _scheduledEffects[i];

                if (turnsRemaining <= 0)
                {
                    effect.ApplyEffect(this);
                    _scheduledEffects.RemoveAt(i);
                }
                else
                {
                    _scheduledEffects[i] = (effect, turnsRemaining - 1);
                }
            }
        }

        public void MarkAsShadowEnemy()
        {
            IsShadowEnemy = true;
        }

        private void UpdateSpeed()
        {
            if (_modifierDuration > 0)
            {
                _modifierDuration--;
                if (_modifierDuration == 0)
                {
                    _currentSpeedModifier = 0;
                }
            }
        }

        public void UpdateStrategy(GameState gameState)
        {
            var map = gameState.Map;
            var objectiveTile = map.GetObjectiveTile();

            if (IsCloseToObjective(objectiveTile))
            {
                IsShadowEnemy = false;
                SetStrategy(new SpeedPrioritizationStrategy());
                RetrievePath(gameState);
                return;
            }

            if (IsHealthLow())
            {
                IsShadowEnemy = false;
                SetStrategy(new SurvivalStrategy());
                RetrievePath(gameState);
                return;
            }

            if (IsInTurretRange(map.Towers))
            {
                if (!(CurrentStrategy is ThreatAvoidanceStrategy))
                {
                    IsShadowEnemy = false;
                    SetStrategy(new ThreatAvoidanceStrategy());
                    RetrievePath(gameState);
                }
                return;
            }

            if (IsShadowEnemy && !(CurrentStrategy is ShadowStrategy))
            {
                SetStrategy(new ShadowStrategy());
                RetrievePath(gameState);
                return;
            }
        }

        private bool IsCloseToObjective(PathPoint objective)
        {
            return CalculateManhattanDistance(X, Y, objective.X, objective.Y) <= closeDistanceThreshold;
        }

        private bool IsHealthLow()
        {
            return Health < lowHealthThreshold;
        }

        private bool IsInTurretRange(List<Tower> towers)
        {
            foreach (var turret in towers)
            {
                if (CalculateManhattanDistance(X, Y, turret.X, turret.Y) <= turret.Weapon.GetRange() +10)
                {
                    return true;
                }
            }
            return false;
        }

        private int CalculateManhattanDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }

        public bool HasReachedDestination()
        {
            return IsAtFinalDestination;
        }

        public void HandleDestination(MainObject mainObject, GameState gameState)
        {
            mainObject.DecreaseHealth(5);

            gameState.HandleEnemyDeath(this);
        }

        public void MoveTowardsNextWaypoint(GameState gameState)
        {
            UpdateScheduledEffects();
            UpdateSpeed();
            UpdateStrategy(gameState);

            if (IsAtFinalDestination) return;

            if (Path == null || Path.Count == 0)
            {
                return;
            }

            var nextWaypoint = Path.Peek();

            if (nextWaypoint.Type == TileType.Objective)
            {
                IsAtFinalDestination = true;
            }

            MoveTo(nextWaypoint, gameState);

            if (X == nextWaypoint.X && Y == nextWaypoint.Y)
            {
                Path.Dequeue();
            }
        }

        private void MoveTo(PathPoint waypoint, GameState gameState)
        {
            int currentSpeed = GetCurrentSpeed();

            if (currentSpeed <= 0)
            {
                return;
            }

            var nextTile = gameState.Map.GetTile(waypoint.X, waypoint.Y);
            if (nextTile == null || nextTile.Type == TileType.Turret)
            {
                return;
            }

            int deltaX = waypoint.X - X;
            int deltaY = waypoint.Y - Y;
            int stepX = deltaX != 0 ? Math.Sign(deltaX) : 0;
            int stepY = deltaY != 0 ? Math.Sign(deltaY) : 0;

            if (Math.Abs(deltaX) > Math.Abs(deltaY))
            {
                stepY = 0;
            }
            else
            {
                stepX = 0;
            }

            while (currentSpeed > 0 && (X != waypoint.X || Y != waypoint.Y))
            {
                if (stepX != 0)
                {
                    if (gameState.Map.GetTile(X + stepX, Y)?.Type != TileType.Turret)
                    {
                        X += stepX;
                        currentSpeed--;
                        ProcessTile(gameState);
                    }
                }

                if (stepY != 0 && currentSpeed > 0)
                {
                    if (gameState.Map.GetTile(X, Y + stepY)?.Type != TileType.Turret)
                    {
                        Y += stepY;
                        currentSpeed--;
                        ProcessTile(gameState);
                    }
                }
            }
        }
        private void ProcessTile(GameState gameState)
        {
            var currentTile = gameState.Map.GetTile(X, Y);

            if (currentTile != null)
            {
                if (_lastTilePosition.x != X || _lastTilePosition.y != Y)
                {
                    currentTile.ApplyEffect(this);
                    _lastTilePosition = (X, Y);
                }
            }
        }

        public void TakeDamage(int damage, GameState gameState)
        {
            if (IsDead())
            {             
                return;
            }
            Health -= damage;

            if (IsDead())
            {
                gameState.HandleEnemyDeath(this); 
            }
        }

        public bool IsDead() => Health <= 0;

        public int DistanceTo(Tower tower) => Math.Abs(tower.X - X) + Math.Abs(tower.Y - Y);

        public void IncreaseHealth(int amount)
        {
            Health += amount;
        }

        public void IncreaseSpeed(int amount)
        {
            Speed += amount;
        }

        public int GetCurrentSpeed()
        {
            return Speed + _currentSpeedModifier;
        }
    }
}
