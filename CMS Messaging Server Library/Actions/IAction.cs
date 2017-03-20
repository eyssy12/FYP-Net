namespace CMS.Messaging.Server.Library.Actions
{
    public interface IAction<in T>
    {
        void Run(T item);
    }
}