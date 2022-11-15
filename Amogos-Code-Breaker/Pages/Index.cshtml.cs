using Amogos_Code_Breaker.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text;
using System.Security.Cryptography;

namespace Amogos_Code_Breaker.Pages
{
    public class IndexModel : PageModel
    {
        private HttpClient httpClient = new HttpClient();

        public string url = "https://exs-htf2022-api.azurewebsites.net/api/challenges/";

        public string AuthorizationKey = "5ee299a1-affc-48de-a7ff-d264477e2d9e";

        public string challenge_1_response = "";

        public Challenges? challenges = new Challenges();

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        private HttpClient GetHttpClient()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", AuthorizationKey);
            return httpClient;
        }

        public void OnPostLoadChallenges()
        {
            Debug.WriteLine("Load challenges called");
            challenges = GetHttpClient().GetFromJsonAsync<Challenges>(url + "locations").Result;
            Debug.WriteLine($"data: {challenges?.challenges?.Count}");
        }

        #region Challenge_1
        public async void OnPostStartChallenge1()
        {
            Challenge_1? challenge1 = await GetHttpClient().GetFromJsonAsync<Challenge_1>(url + "azkaban");

            double angle = (180 / Math.PI) * CalculateAngle(challenge1);

            var postResponse = await GetHttpClient().PostAsync(
                url + "azkaban",
                new StringContent(JsonSerializer.Serialize(new Answer() { answer = angle }), Encoding.UTF8, "application/json")
                );

            if (postResponse.IsSuccessStatusCode)
            {
                Debug.WriteLine("Success");
                Response r = JsonSerializer.Deserialize<Response>(postResponse.Content.ReadAsStringAsync().Result);
                Debug.WriteLine(r.keyPart);
            }
        }

        public double CalculateAngle(Challenge_1? challenge_1)
        {
            double height = challenge_1.monsterHeight - challenge_1.heroWeaponHeight - challenge_1.monsterNeckDistance;
            return Math.Atan(height / challenge_1.heroDistanceFromMonster);
        }
        #endregion

        #region Challenge_2
        public async void OnPostStartChallenge2()
        {
            Challenge_2? challenge2 = await GetHttpClient().GetFromJsonAsync<Challenge_2>(url + "statue-of-slytherin");

            challenge2.key = AppendZeros(challenge2.key);
            challenge2.iv = AppendZeros(challenge2.iv);

            string result = Decrypt(challenge2.spell, challenge2.key, challenge2.iv);

            Debug.WriteLine(result);

            //var postResponse = await GetHttpClient().PostAsync(
            //    url + "statue-of-slytherin",
            //    new StringContent(JsonSerializer.Serialize(new Answer() { answer =  }), Encoding.UTF8, "application/json")
            //    );

            //if (postResponse.IsSuccessStatusCode)
            //{
            //    Debug.WriteLine("Success");
            //    Response r = JsonSerializer.Deserialize<Response>(postResponse.Content.ReadAsStringAsync().Result);
            //    Debug.WriteLine(r.keyPart);
            //}
        }

        private string Decrypt(string input, string iv, string key)
        {
            AesCryptoServiceProvider _aes = new AesCryptoServiceProvider();
            _aes.KeySize = 128;
            _aes.Key = ASCIIEncoding.ASCII.GetBytes(key);
            _aes.IV = ASCIIEncoding.ASCII.GetBytes(iv);
            _aes.Mode = CipherMode.CBC;

            ICryptoTransform  _crypto = _aes.CreateDecryptor(_aes.Key, _aes.IV);
            byte[] decrypted = _crypto.TransformFinalBlock(ASCIIEncoding.ASCII.GetBytes(input),
                                            0, ASCIIEncoding.ASCII.GetBytes(input).Length);
            _crypto.Dispose();

            return ASCIIEncoding.ASCII.GetString(decrypted);
        }

        private string AppendZeros(string value)
        {
            while(value.Length < 16)
            {
                value += "0";
            }

            return value;
        }
        #endregion

        #region Challenge_3
        public void OnPostStartChallenge3()
        {

        }
        #endregion
    }
}