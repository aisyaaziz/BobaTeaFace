using BobaTeaFace.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace BobaTeaFace.Controllers
{
    public class PrototypesController : Controller
    {
        string APIKEY = string.Empty;
        public PrototypesController(IConfiguration conf)
        {
            APIKEY = conf.GetSection("OPENAI_API_KEY").Value;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateImage([FromBody] DalleEAPIModelInput input)
        {
            // create a response object
            var resp = new DalleEAPIModelResponseModel();
            using (var client = new HttpClient())
            {
                // clear the default headers to avoid issues
                client.DefaultRequestHeaders.Clear();

                // add header authorization and use your API KEY
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", APIKEY);

                string jsonInput = JsonConvert.SerializeObject(input);
                //  call the  api using post method and set the content type to application/json
                var Message = await client.PostAsync("https://api.openai.com/v1/images/generations",
                    new StringContent(jsonInput, Encoding.UTF8, "application/json"));

                // if result OK
                // read the content and deserialize it using the Response Model
                // then return the response object
                if (Message.IsSuccessStatusCode)
                {

                    var content = await Message.Content.ReadAsStringAsync();
                    resp = JsonConvert.DeserializeObject<DalleEAPIModelResponseModel>(content);
                }
            }


            return Json(resp);
        }


        public IActionResult Scenario()
        {
            return View();
        }
    }
}
