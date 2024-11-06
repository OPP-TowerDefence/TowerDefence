using TowerDefense.Enums;
using TowerDefense.Models.Towers;
using TowerDefense.Utils;
using TowerDefense.Models.Strategies;

namespace TowerDefense.Models.Enemies
{
    public abstract class Enemy : Unit
    {
        public Guid Id { get; set; }
        public int Health { get; set; }
        public int RewardValue { get; set; } = 10;
        public int Speed { get; set; }
        public Queue<PathPoint> Path { get; set; }
        private int _currentSpeedModifier = 0;
        private int _modifierDuration = 0;
        private (int x, int y) _lastTilePosition;
        private bool IsAtFinalDestination { get; set; } = false;

        public IPathStrategy CurrentStrategy { get; private set; }
        public bool IsShadowEnemy { get; private set; } = false;
        public abstract EnemyTypes Type { get; }

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

            // 1. Check if close to the objective - switch to SpeedPrioritizationStrategy
            if (IsCloseToObjective(objectiveTile))
            {
                IsShadowEnemy = false;
                SetStrategy(new SpeedPrioritizationStrategy());
                RetrievePath(gameState);
                return;
            }

            // 2. Check if health is low - switch to SurvivalStrategy
            if (IsHealthLow())
            {
                IsShadowEnemy = false;
                SetStrategy(new SurvivalStrategy());
                RetrievePath(gameState);
                return;
            }

            // 3. Check if within turret range - switch to ThreatAvoidanceStrategy
            if (IsInTurretRange(map.Towers))
            {
                IsShadowEnemy = false;
                SetStrategy(new ThreatAvoidanceStrategy());
                RetrievePath(gameState);
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
            const int closeDistanceThreshold = 15; // Define "close" as within 5 tiles (adjust as needed)
            return CalculateManhattanDistance(X, Y, objective.X, objective.Y) <= closeDistanceThreshold;
        }
        private bool IsHealthLow()
        {
            const int lowHealthThreshold = 20; // Define low health as below 30 (adjust as needed)
            return Health < lowHealthThreshold;
        }
        private bool IsInTurretRange(List<Tower> towers)
        {
            foreach (var turret in towers)
            {
                if (CalculateManhattanDistance(X, Y, turret.X, turret.Y) <= turret.Weapon.GetRange())
                {
                    return true; // Enemy is within range of at least one turret
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

        public void MoveTowardsNextWaypoint(GameState gameState)
        {
            UpdateSpeed();
            UpdateStrategy(gameState);

            // Stop any further movement if the enemy is already at the final destination
            if (IsAtFinalDestination) return;

            // Check if the path is empty (for ShadowStrategy, it may be waiting for a target)
            if (Path == null || Path.Count == 0)
            {
                // Stay in a waiting state until a path is available
                return;
            }
            // Proceed with movement if path is available
            var nextWaypoint = Path.Peek();
            if (nextWaypoint.Type == TileType.Objective)
            {
                IsAtFinalDestination = true;
            }
            // Move towards the next waypoint
            MoveTo(nextWaypoint, gameState);

            // If the enemy has reached the current waypoint, dequeue to move to the next
            if (X == nextWaypoint.X && Y == nextWaypoint.Y)
            {
                Path.Dequeue();
            }
        }

        private void MoveTo(PathPoint waypoint, GameState gameState)
        {
            int currentSpeed = GetCurrentSpeed();

            // Prevent movement if current speed is 0
            if (currentSpeed <= 0)
            {
                return;
            }

            // Check if the waypoint is valid
            var nextTile = gameState.Map.GetTile(waypoint.X, waypoint.Y);
            if (nextTile == null || nextTile.Type == TileType.Turret) // Ensure we don’t move to a turret tile
            {
                return; // Invalid movement; stop here
            }

            // Calculate the direction and distance to the waypoint
            int deltaX = waypoint.X - X;
            int deltaY = waypoint.Y - Y;

            // Determine the step directions (1, 0, or -1 for both X and Y)
            int stepX = deltaX != 0 ? Math.Sign(deltaX) : 0;
            int stepY = deltaY != 0 ? Math.Sign(deltaY) : 0;

            // Ensure only horizontal or vertical movement
            if (Math.Abs(deltaX) > Math.Abs(deltaY))
            {
                stepY = 0; // No vertical movement
            }
            else
            {
                stepX = 0; // No horizontal movement
            }

            // Loop to simulate step-by-step movement
            while (currentSpeed > 0 && (X != waypoint.X || Y != waypoint.Y))
            {
                // Move along the X-axis if necessary
                if (stepX != 0)
                {
                    // Check if the next move is valid
                    if (gameState.Map.GetTile(X + stepX, Y)?.Type != TileType.Turret)
                    {
                        X += stepX;
                        currentSpeed--;
                        ProcessTile(gameState); // Process the tile after each movement
                    }
                }

                // Move along the Y-axis if necessary
                if (stepY != 0 && currentSpeed > 0)
                {
                    // Check if the next move is valid
                    if (gameState.Map.GetTile(X, Y + stepY)?.Type != TileType.Turret)
                    {
                        Y += stepY;
                        currentSpeed--;
                        ProcessTile(gameState); // Process the tile after each movement
                    }
                }
            }
        }
        private void ProcessTile(GameState gameState)
        {
            var currentTile = gameState.Map.GetTile(X, Y);  // Use the Map's GetTile method to retrieve the current tile

            if (currentTile != null)
            {
                // If the enemy has moved to a new tile
                if (_lastTilePosition.x != X || _lastTilePosition.y != Y)
                {
                    // Apply the tile's effect only once per tile
                    currentTile.Effect?.ApplyEffect(this);
                    // Update the last tile position after processing
                    _lastTilePosition = (X, Y);
                }
            }
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
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
            return Speed + _currentSpeedModifier;  // Return base speed plus any current modifiers
        }
    }
}
