using Newtonsoft.Json;

namespace Idams.WebApi.Utils.Extensions
{
    public static class DistributeCacheExtentions
    {
        public static byte[] ToBytes(this object model)
        {
            return System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model));
        }

        public static T To<T>(this byte[] model)
        {
            if (model is null)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(System.Text.Encoding.UTF8.GetString(model));
        }
    }
}