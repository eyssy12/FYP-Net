namespace TestApi.Library.Security
{
    using System.Collections.Generic;
    using System.Linq;

    public class TokenStorageService
    {
        protected readonly IList<StorageToken> ReusableTokens;

        public TokenStorageService()
        {
            this.ReusableTokens = new List<StorageToken>();
        }

        public void AddToken(StorageToken token)
        {
            if (!this.ReusableTokens.Any(s => s.EntityId == token.EntityId))
            {
                this.ReusableTokens.Add(token);
            }
        }

        public StorageToken GetToken(string entityId)
        {
            return this.ReusableTokens.SingleOrDefault(s => s.EntityId == entityId);
        }

        public void Replace(StorageToken newToken)
        {
            StorageToken existing = this.ReusableTokens.Where(s => s.EntityId == newToken.EntityId).First();

            int index = this.ReusableTokens.IndexOf(existing);

            if (index != -1)
            {
                this.ReusableTokens[index] = newToken;
            }
        }
    }
}