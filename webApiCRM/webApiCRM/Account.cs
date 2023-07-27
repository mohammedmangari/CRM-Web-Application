using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Runtime.InteropServices;

namespace webApiCRM
{
	public class Account
	{

        [JsonProperty("accountid")]
        public string accountid { get; set; } =  Guid.NewGuid().ToString();

        [JsonProperty("name")]
        public string? name { get; set; }

        [JsonProperty("creditonhold")]
		public Boolean? creditonhold { get; set; }

		[JsonProperty("description")]
		public string? description { get; set; }

		[JsonProperty("revenue")]
        public long? revenue { get; set; }

		[JsonProperty("address1_composite")]
		public string? address1_composite { get; set; }

        [JsonProperty("websiteurl")]
        public string? websiteurl { get; set; }





    }
}
