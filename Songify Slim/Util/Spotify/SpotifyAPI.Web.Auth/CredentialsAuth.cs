using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Songify_Slim.Util.Spotify.SpotifyAPI.Web.Models;

namespace Songify_Slim.Util.Spotify.SpotifyAPI.Web.Auth
{
  public class CredentialsAuth
  {
    public string ClientSecret { get; set; }

    public string ClientId { get; set; }

    public ProxyConfig ProxyConfig { get; set; }

    public CredentialsAuth(string clientId, string clientSecret)
    {
      ClientId = clientId;
      ClientSecret = clientSecret;
    }

    public async Task<Token> GetToken()
    {
      string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(ClientId + ":" + ClientSecret));

      List<KeyValuePair<string, string>> args = new()
      {new KeyValuePair<string, string>("grant_type", "client_credentials")
      };

      HttpClientHandler handler = ProxyConfig.CreateClientHandler(ProxyConfig);
      HttpClient client = new(handler);
      client.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");
      HttpContent content = new FormUrlEncodedContent(args);

      HttpResponseMessage resp = await client.PostAsync("https://accounts.spotify.com/api/token", content);
      string msg = await resp.Content.ReadAsStringAsync();

      return JsonConvert.DeserializeObject<Token>(msg);
    }
  }
}
