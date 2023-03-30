using BobaTeaFace.Models;
using BobaTeaFace.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using static Humanizer.In;

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

        [HttpPost]
        public async Task<IActionResult> Scenario([FromBody] ScenarioViewModel model)
        {
            string actualScenario = "";

            var resp = new ChatGPTAPIModelResponseModel();
            using (var client = new HttpClient())
            {
                // clear the default headers to avoid issues
                client.DefaultRequestHeaders.Clear();

                // add header authorization and use your API KEY
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", APIKEY);

                string gender = "boy";
                if (model.Gender == 20)
                {
                    gender = "girl";
                }

                ChatGPTAPIModelInput dalleInput = new ChatGPTAPIModelInput();
                dalleInput.model = "gpt-3.5-turbo";
                dalleInput.max_tokens = 2048;

                ChatGPTAPIModelInputMessage msg = new ChatGPTAPIModelInputMessage();
                msg.role = "user";
                msg.content = "Generate a less than 400-character short prompt to give to DALL_E to generate a non-animated image to show to an autistic child (" + gender + ") a scenario about " + model.Scenario + " with minimal distractions.";
                dalleInput.messages = new List<ChatGPTAPIModelInputMessage>();
                dalleInput.messages.Add(msg);
                
                string jsonInput = JsonConvert.SerializeObject(dalleInput);
                //  call the  api using post method and set the content type to application/json
                var Message = await client.PostAsync("https://api.openai.com/v1/chat/completions",
                    new StringContent(jsonInput, Encoding.UTF8, "application/json"));

                // if result OK
                // read the content and deserialize it using the Response Model
                // then return the response object
                if (Message.IsSuccessStatusCode)
                {

                    var content = await Message.Content.ReadAsStringAsync();
                    resp = JsonConvert.DeserializeObject<ChatGPTAPIModelResponseModel>(content);
                }
            }


            // create a response object
            var dallEResponse = new DalleEAPIModelResponseModel();
            using (var client = new HttpClient())
            {
                // clear the default headers to avoid issues
                client.DefaultRequestHeaders.Clear();

                // add header authorization and use your API KEY
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", APIKEY);

                DalleEAPIModelInput dalleInput = new DalleEAPIModelInput();
                dalleInput.n = 1;
                dalleInput.size = "256x256";
                if (resp.choices.Count > 0)
                {
                    dalleInput.prompt = resp.choices.FirstOrDefault().message.content;
                } else
                {
                    dalleInput.prompt = model.Scenario;
                }
                actualScenario = dalleInput.prompt;
                string jsonInput = JsonConvert.SerializeObject(dalleInput);
                //  call the  api using post method and set the content type to application/json
                var Message = await client.PostAsync("https://api.openai.com/v1/images/generations",
                    new StringContent(jsonInput, Encoding.UTF8, "application/json"));

                // if result OK
                // read the content and deserialize it using the Response Model
                // then return the response object
                if (Message.IsSuccessStatusCode)
                {

                    var content = await Message.Content.ReadAsStringAsync();
                    dallEResponse = JsonConvert.DeserializeObject<DalleEAPIModelResponseModel>(content);
                }
            }

            ScenarioResponseViewModel vm = new ScenarioResponseViewModel();
            vm.scenario = actualScenario;
            if (dallEResponse != null)
            {
                vm.data = dallEResponse.data;
            }
            return Json(vm);
        }


        public IActionResult Scenario2()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Scenario2([FromBody] ScenarioViewModel model)
        {
            string actualScenario = "";
            var resp = new ChatGPTAPIModelResponseModel();
            using (var client = new HttpClient())
            {
                // clear the default headers to avoid issues
                client.DefaultRequestHeaders.Clear();

                // add header authorization and use your API KEY
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", APIKEY);

                string gender = "boy";
                if (model.Gender == 20)
                {
                    gender = "girl";
                }

                ChatGPTAPIModelInput dalleInput = new ChatGPTAPIModelInput();
                dalleInput.model = "gpt-3.5-turbo";


                string tempmessage = @"Generate 5W and 1 H questions based on the scenario. Remember it is for Autistic children so keep English simple with limited vocabulary. 
Scenario: A kid which is a happy [" + gender + "] that can be seen in one image in the following scenario: " + model.Scenario;

                tempmessage = tempmessage + @"The output should be in the format below as I m feeding it to code so just answer as below no extra words:

                Prompt for DALL E : “_____”

                5W and 1H Questions:
                Who Question: 
                What Question: 
                Where Question:
                When Question: 
                Why Question:
                How Question: 

                5W and 1H Answers:
                Who Answers: 
                What Answers: 
                Where Answers:
                When Answers: 
                Why Answers:
                How Answers:";


                ChatGPTAPIModelInputMessage msg = new ChatGPTAPIModelInputMessage();
                msg.role = "user";
                msg.content = tempmessage;
                dalleInput.messages = new List<ChatGPTAPIModelInputMessage>();
                dalleInput.messages.Add(msg);

                string jsonInput = JsonConvert.SerializeObject(dalleInput);
                //  call the  api using post method and set the content type to application/json
                var Message = await client.PostAsync("https://api.openai.com/v1/chat/completions",
                    new StringContent(jsonInput, Encoding.UTF8, "application/json"));

                // if result OK
                // read the content and deserialize it using the Response Model
                // then return the response object
                if (Message.IsSuccessStatusCode)
                {

                    var content = await Message.Content.ReadAsStringAsync();
                    resp = JsonConvert.DeserializeObject<ChatGPTAPIModelResponseModel>(content);
                }
            }

            string message = "";
            if (resp != null)
            {
                if (resp.choices.Count > 0)
                {
                    message = resp.choices.FirstOrDefault().message.content;
                }
            }
            string thescenario = "";
            Regex regex = new Regex("\"([^\"]*)\"");
            Match match = regex.Match(message);
            if (match.Success)
            {
                thescenario = match.Groups[1].Value;
            }

            // create a response object
            var dallEResponse = new DalleEAPIModelResponseModel();
            using (var client = new HttpClient())
            {
                // clear the default headers to avoid issues
                client.DefaultRequestHeaders.Clear();

                // add header authorization and use your API KEY
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", APIKEY);

                DalleEAPIModelInput dalleInput = new DalleEAPIModelInput();
                dalleInput.n = 1;
                dalleInput.size = "256x256";
                dalleInput.prompt = thescenario;
                actualScenario = thescenario;
                string jsonInput = JsonConvert.SerializeObject(dalleInput);
                //  call the  api using post method and set the content type to application/json
                var Message = await client.PostAsync("https://api.openai.com/v1/images/generations",
                    new StringContent(jsonInput, Encoding.UTF8, "application/json"));

                // if result OK
                // read the content and deserialize it using the Response Model
                // then return the response object
                if (Message.IsSuccessStatusCode)
                {

                    var content = await Message.Content.ReadAsStringAsync();
                    dallEResponse = JsonConvert.DeserializeObject<DalleEAPIModelResponseModel>(content);
                }
            }


            ScenarioResponseViewModel vm = new ScenarioResponseViewModel();
            vm.scenario = message;
            if (dallEResponse != null)
            {
                vm.data = dallEResponse.data;
            }
            return Json(vm);
        }
    }
}
