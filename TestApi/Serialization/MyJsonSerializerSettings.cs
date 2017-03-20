namespace TestApi.Serialization
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class MyJsonSerializerSettings : JsonSerializerSettings
    {
        public MyJsonSerializerSettings()
        {
            this.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            this.NullValueHandling = NullValueHandling.Ignore;
            this.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}