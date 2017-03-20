namespace CMS.Dashboard.Test.Services
{
    public interface INotify<TNotification>
        where TNotification : class
    {
        void SendNotification(TNotification message);
    }
}