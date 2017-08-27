using Newtonsoft.Json;
using OMSToSlack.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OMSToSlack
{
    public static class SlackHelper
    {
        public static async Task SendSlackMessage(string slackChannel, string message)
        {
            var slackUri = "https://requestb.in/1hoo74a1";
            var slackUsername = "Alerts";

            using (var client = new HttpClient())
            {
                System.Diagnostics.Debug.WriteLine($"{slackChannel} - {message}");

                var slackPayload = new SlackMessage() { Text = message, Channel = slackChannel, Username = slackUsername, LinkNames = true };
                var hook = new StringContent(JsonConvert.SerializeObject(slackPayload), Encoding.UTF8, "application/json");
                var resp = await client.PostAsync(slackUri, hook);
            }
        }
    }
}
