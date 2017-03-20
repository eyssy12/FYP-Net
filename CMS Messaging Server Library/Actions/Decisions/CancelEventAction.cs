namespace CMS.Messaging.Server.Library.Actions.Decisions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using agsXMPP;
    using CMS.Library.Data;
    using CMS.Library.Extensions;
    using CMS.Library.Factories;
    using Communications;
    using Configuration.Data.Models;
    using Models;
    using Newtonsoft.Json;
    using Xmpp;
    using Xmpp.Contract.Resolvers;
    using AgsMessage = agsXMPP.protocol.client.Message;

    public class CancelEventAction : ActionBase, IAction<ParsedGcmMessageStateHolder>
    {
        protected readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        protected readonly IXmppConnectionClient XmppClient;
        protected readonly JsonSerializerSettings Settings;
        protected readonly Random Random;

        protected readonly Jid ToGcmJid;
        protected readonly Jid FromMessagingServerJid;

        public CancelEventAction(
            IFactory factory, 
            IXmppConnectionClient xmppClient,
            string identifierKey = Protocol.CancelEvent)
            : base(factory, identifierKey)
        {
            if (xmppClient == null)
            {
                throw new ArgumentNullException(nameof(xmppClient), "An XMPP client is missing, the action would not be able to send messages to GCM");
            }

            this.XmppClient = xmppClient;
            this.Settings = new JsonSerializerSettings
            {
                ContractResolver = new UnderscoreBetweenMultiWordIdentifierPropertyContractResolver() // TODO: this should not be instantiated like so
            };

            this.Random = new Random(DateTime.UtcNow.Millisecond);
            this.ToGcmJid = new Jid("devices@gcm.googleapis.com");
            this.FromMessagingServerJid = new Jid(this.XmppClient.MyJabberID);
        }

        public void Run(ParsedGcmMessageStateHolder item)
        {
            if (item.Action == this.IdentifierKey)
            {
                Console.WriteLine("-> (" + DateTime.UtcNow.ToString() + ") - Cancel event action initiated");

                using (ISimpleRepository<Event> eventRepository = this.Factory.Create<ISimpleRepository<Event>>())
                using (ISimpleRepository<AspNetUser> userRepository = this.Factory.Create<ISimpleRepository<AspNetUser>>())
                using (ISimpleRepository<Timetable> timetableRepository = this.Factory.Create<ISimpleRepository<Timetable>>())
                {
                    int eventId = int.Parse(item.Value);
                    Event @event = eventRepository.Get().SingleOrDefault(s => s.Id == eventId);
                    DateTime timestamp = this.UnixEpoch + TimeSpan.FromMilliseconds(long.Parse(item.Timestamp));

                    AspNetUser user = this.RetrieveUser(userRepository, item.EntityId);
                    CancelledEvent cancelledEvent = this.CreateAnEventCancellation(eventId, timestamp, user.UserName);

                    this.AddCancelationToStore(eventRepository, @event, cancelledEvent);
                    this.RetrieveMobileClientsOfAffectedEntities(timetableRepository, @event).ForEach(client =>
                    {
                        object contents = new
                        {
                            MessageId = Random.Next(1, int.MaxValue - 1).ToString(),
                            To = client.Token,
                            Category = "com.eyssyapps.fypcms",
                            TimeToLive = 86400,
                            Data = new
                            {
                                Action = this.IdentifierKey,
                                Value = @event.Id.ToString(),
                                Timestamp = cancelledEvent.Timestamp,
                                CancelledBy = cancelledEvent.CancelledBy
                            }
                        };

                        AgsMessage message = this.ToGcmJid.CreateNotificationMessage(this.FromMessagingServerJid, contents, settings: this.Settings);
                        this.XmppClient.Send(message);
                    });
                }
            }
        }

        protected CancelledEvent CreateAnEventCancellation(int eventId, DateTime timestamp, string cancelledBy)
        {
            return new CancelledEvent
            {
                CancellationEventId = eventId,
                Timestamp = timestamp,
                CancelledBy = cancelledBy
            };
        }

        protected void AddCancelationToStore(ISimpleRepository<Event> eventRepository, Event affectedEvent, CancelledEvent cancellation)
        {
            affectedEvent.CancelledEvents.Add(cancellation);
            eventRepository.SaveChanges();
        }

        protected AspNetUser RetrieveUser(ISimpleRepository<AspNetUser> userRepository, string entityId)
        {
            return userRepository
                .Get()
                .SingleOrDefault(s => s.Id == entityId);
        }

        protected IEnumerable<GcmMobileClient> RetrieveMobileClientsOfAffectedEntities(ISimpleRepository<Timetable> timetableRepository, Event affectedEvent)
        {
            Timetable affectedTimetable = timetableRepository
                .Get()
                .SingleOrDefault(s => s.Events.Any(e => e.Id == affectedEvent.Id));

            return affectedTimetable
                .Class
                .Students
                .SelectMany(s =>s.StudentPersons)
                .Select(s => s.Person)
                .SelectMany(s => s.GcmMobileClients);
        }
    }
}