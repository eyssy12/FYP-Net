namespace CMS.Library.Factories
{
    public interface IFactory
    {
        TInstance Create<TInstance>();
    }
}