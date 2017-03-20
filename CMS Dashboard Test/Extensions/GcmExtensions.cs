namespace CMS.Dashboard.Test.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Linq;
    using PushSharp.Google;

    public static class GcmExtensions
    {
        public static GcmNotification CreateNotification(this IEnumerable<string> registrationIds, JObject data, GcmNotificationPriority priority)
        {
            GcmNotification message = new GcmNotification
            {
                RegistrationIds = registrationIds.ToList(),
                Data = data,
                Priority = priority
            };

            return message;
        }
    }
}