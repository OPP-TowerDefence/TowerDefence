namespace TowerDefense.Interfaces.Visitor
{
    public interface IVisitable
    {
        public void Accept(IEffectVisitor visitor);
    }
}
