namespace TestApi.Library
{
    public static class Protocol
    {
        public const string UserType = "user_type",
            StudentId = "student_id",
            LecturerId = "lecturer_id",
            Authenticated = "authenticated",
            EntityId = "entity_id",
            IdToken = "id_token",
            IdTokenExpiry = "id_token_expiry",
            AccessToken = "access_token",
            AccessTokenExpiry = "access_token_expiry",
            Contents = "contents",
            MessageType = "message_type",
            MessageTypeNotification = "notification",
            TimetableChange = "timetable_change",
            TimetableChangeCancelledEvent = "timetable_change_cancelled_events",
            CancelledEventIds = "cancelled_event_ids",
            ModifiedEvents = "modified_events",
            NewEvents = "new_events",
            RemovedEvents = "removed_events";
    }
}