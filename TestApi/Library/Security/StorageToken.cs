namespace TestApi.Library.Security
{
    using System;

    public class StorageToken
    {
        private string entityId,
            token;

        private DateTime expirationTime;

        public StorageToken(string entityId, string token, DateTime expirationTime)
        {
            this.entityId = entityId;
            this.token = token;
            this.expirationTime = expirationTime;
        }

        public string EntityId
        {
            get { return this.entityId; }
        }

        public string Token
        {
            get { return this.token; }
        }
        
        public DateTime ExpirationTime
        {
            get { return this.expirationTime; }
        }
    }
}