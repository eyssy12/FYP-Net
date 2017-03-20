namespace CMS.Messaging.Server.Library.Communications
{
    public interface IConnectionClient
    {
        void Connect();

        void Close();

        void Send(object item);
    }
}