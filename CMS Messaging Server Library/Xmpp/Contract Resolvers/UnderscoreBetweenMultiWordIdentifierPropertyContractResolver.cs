namespace CMS.Messaging.Server.Library.Xmpp.Contract.Resolvers
{
    using System.Text.RegularExpressions;
    using Newtonsoft.Json.Serialization;

    public class UnderscoreBetweenMultiWordIdentifierPropertyContractResolver : DefaultContractResolver
    {
        protected const string DefaultRegularExpression = @"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])",
            Underscore = "_";

        protected readonly Regex Regex;

        private bool toLowerCase;

        public UnderscoreBetweenMultiWordIdentifierPropertyContractResolver(
            string regex = UnderscoreBetweenMultiWordIdentifierPropertyContractResolver.DefaultRegularExpression, 
            bool toLowerCase = true)
        {
            this.toLowerCase = toLowerCase;
            this.Regex = new Regex(regex, RegexOptions.IgnorePatternWhitespace);
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            string result = this.Regex.Replace(propertyName, UnderscoreBetweenMultiWordIdentifierPropertyContractResolver.Underscore);

            if (this.toLowerCase)
            {
                return result.ToLower();
            }

            return result;
        }
    }
}