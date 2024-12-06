using TowerDefense.Interfaces;
using TowerDefense.Interfaces.Visitor;
using TowerDefense.Models.MainObjectStates;

namespace TowerDefense.Models
{
    public class MainObject : Unit, IVisitable
    {
        public int Health { get; set; }

        private const int _maxHealth = 100;
        private const string _stateGifBase = "http://localhost:7041/MainObject";
        private IMainObjectState _state;

        public MainObject(int x, int y, int initialHealth = _maxHealth) : base(x, y)
        {
            Health = initialHealth;
            _state = new NormalState();
        }

        public void Accept(IEffectVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void ChangeState(IMainObjectState state)
        {
            _state = state;
        }

        public void DealDamage(int damage)
        {
            _state.DealDamage(this, damage);
        }

        public void Repair(int health)
        {
            _state.Repair(this, health);
        }

        public string GetStateGif()
        {
            return _stateGifBase + _state.GetStateGif();
        }

        public bool IsDestroyed()
        {
            return _state.IsDestroyed();
        }

        public int GenerateResources()
        {
            return _state.GenerateResources();
        }
    }
}
