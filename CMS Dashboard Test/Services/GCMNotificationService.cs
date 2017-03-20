namespace CMS.Dashboard.Test.Services
{
    using PushSharp.Google;

    public class GCMNotificationService : INotifyService<GcmNotification>
    {
        protected readonly GcmServiceBroker Broker;

        public GCMNotificationService()
        {
            // TODO: add consts to resources?
            GcmConfiguration config = new GcmConfiguration("334770348820", "AIzaSyDF58nZS8k8KzA5lo4l2S4lViYFBhRq7jA", null);

            this.Broker = new GcmServiceBroker(config);
        }

        public void Start()
        {
            this.Broker.Start();
        }

        public void Stop()
        {
            this.Broker.Stop();
        }

        public void SendNotification(GcmNotification message)
        {
            if (message == null)
            {
                return;
            }

            this.Broker.QueueNotification(message);
        }
    }
}