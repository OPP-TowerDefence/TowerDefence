﻿using System.Collections;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Enemies
{
    public class EnemyGroup : IEnemyComponent
    {
        private readonly List<IEnemyComponent> _children = [];

        public IEnumerable<IEnemyComponent> Children => _children;

        public void MoveTowardsNextWaypoint(GameState gameState)
        {
            foreach (var child in _children.ToList())
            {
                child.MoveTowardsNextWaypoint(gameState);
            }
        }

        public bool HasReachedDestination()
        {
            return _children.All(child => child.HasReachedDestination());
        }

        public bool IsDead()
        {
            return _children.All(child => child.IsDead());
        }

        public void HandleDestination(MainObject mainObject, GameState gameState)
        {
            foreach (var child in _children.ToList())
            {
                child.HandleDestination(mainObject, gameState);
                Remove(child);
            }

            if (IsDead())
            {
                gameState.Map.Enemies.Remove(this);
            }
        }

        public void TakeDamage(int damage, GameState gameState)
        {
            foreach (var child in _children.ToList())
            {
                child.TakeDamage(damage, gameState);

                if (child.IsDead())
                {
                    Remove(child);
                    gameState.Map.Enemies.Remove(child);
                }
            }

            if (IsDead())
            {
                gameState.Map.Enemies.Remove(this);
            }
        }

        public void Add(IEnemyComponent child) => _children.Add(child);

        public void Remove(IEnemyComponent child) => _children.Remove(child);

        public IEnumerator<Enemy> GetEnumerator()
        {
            foreach (var child in _children)
            {
                foreach (var enemy in child)
                {
                    yield return enemy;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
