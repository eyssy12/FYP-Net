namespace CMS.Library.Extensions
{
    using Newtonsoft.Json;

    public static class JsonExtensions
    {
        public static string SerializeAsJson(this object value, Formatting formatting = Formatting.None, JsonSerializerSettings settings = null)
        {
            if (settings == null)
            {
                return JsonConvert.SerializeObject(value);
            }

            return JsonConvert.SerializeObject(value, formatting, settings);
        }
    }
}