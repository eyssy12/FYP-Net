namespace CMS.Dashboard.Test.Serialization
{
    using Newtonsoft.Json;

    public class MyJsonSerializerSettings : JsonSerializerSettings
    {
        public MyJsonSerializerSettings()
        {
            this.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }
    }
}