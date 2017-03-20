namespace CMS.Dashboard.Test.Services
{
    public interface INotifyService<TNotification> : INotify<TNotification>, IService
        where TNotification : class
    {
    }
}