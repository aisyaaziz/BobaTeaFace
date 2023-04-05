using BobaTeaFace.Models;
using BobaTeaFace.ViewModels;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static Humanizer.In;
using Microsoft.AspNetCore.Hosting;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using BobaTeaFace.Data;
using Microsoft.AspNetCore.Http;

namespace BobaTeaFace.Controllers
{
    public class PrototypesController : Controller
    {
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        private string APIKEY = string.Empty;
        private ApplicationDbContext _db;

        public PrototypesController(IConfiguration conf, ApplicationDbContext db, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _db = db;
            APIKEY = conf.GetSection("OPENAI_API_KEY").Value;
            _hostingEnvironment = environment;
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
                msg.content = "Generate a less than 400-character short prompt to give to DALL_E to generate a real life image to show to an autistic child (" + gender + ") a scenario about " + model.Scenario + " with minimal distractions.";
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

                string tempmessage = @"Generate 5W and 1 H questions based on the scenario and generate a prompt for DALL e to generate a real-life image of the scenario. Remember it is for Autistic children so keep English simple with limited vocabulary, also keep images simple and happy with minimal distractions. 
Scenario: A real life happy " + gender + " that can be seen in one image in the following scenario: " + model.Scenario;

                tempmessage = tempmessage + @"The output should be in the format below as I am feeding it to code so just answer as below no extra words:

                Prompt for DALL E : “___”

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
            vm.scenario = vm.scenario.Substring(vm.scenario.IndexOf("\n") + 1);
            vm.scenario = vm.scenario.Substring(vm.scenario.IndexOf("\n") + 1);
            vm.scenario = vm.scenario.Replace("\n", "<br />");
            if (dallEResponse != null)
            {
                vm.data = dallEResponse.data;
            }
            return Json(vm);
        }



        public IActionResult ImageScenario()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImageScenarioPost()
        {
            string actualScenario = "";
            string gender = "boy";
            string scenario = "";
            if (Request.Form.Where(x => x.Key.ToLower() == "gender").Count() > 0)
            {
                KeyValuePair<string, StringValues> formGender = Request.Form.Where(x => x.Key == "gender").FirstOrDefault();
                if (formGender.Value == "20")
                {
                    gender = "girl";
                }
            }
            if (Request.Form.Where(x => x.Key.ToLower() == "scenario").Count() > 0)
            {
                KeyValuePair<string, StringValues> formScenario = Request.Form.Where(x => x.Key == "scenario").FirstOrDefault();
                scenario = formScenario.Value;
            }
            string fileUrl = "https://bobateaapp.theaisyaaziz.com/uploads/";
            IFormFileCollection files = Request.Form.Files;
            if (files.Count() > 0)
            {
                IFormFile file = files.FirstOrDefault();
                string uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                string filePath = Path.Combine(uploads, file.FileName);
                fileUrl = fileUrl + file.FileName;
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            var resp = new ChatGPTAPIModelResponseModel();
            using (var client = new HttpClient())
            {
                // clear the default headers to avoid issues
                client.DefaultRequestHeaders.Clear();

                // add header authorization and use your API KEY
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", APIKEY);


                ChatGPTAPIModelInput dalleInput = new ChatGPTAPIModelInput();
                dalleInput.model = "gpt-3.5-turbo";
                dalleInput.max_tokens = 2048;

                ChatGPTAPIModelInputMessage msg = new ChatGPTAPIModelInputMessage();
                msg.role = "user";
                msg.content = "Generate a less than 400-character short prompt to give to DALL_E to generate a real life image to show to an autistic child (" + gender + ") a scenario about " + scenario + " with minimal distractions.";
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
                }
                else
                {
                    dalleInput.prompt = scenario;
                }
                dalleInput.prompt = dalleInput.prompt.Replace("\"","");
                dalleInput.prompt = fileUrl + "***" + dalleInput.prompt;
            
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

        public IActionResult ChatScenario()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChatScenarioPost([FromBody] ScenarioChatViewModel model)
        {
            string sessionId = model.guid;
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
            }
            IEnumerable<Chat> chats = _db.Chats.Where(x => x.SessionId == sessionId).ToList();
            if (chats.Count() == 0)
            {
                _db.Chats.Add(new Chat { SessionId = sessionId, Role = "system", Content = "Your name is Sunny Yang. You are chatting with an autism kids who is trying to learn to be social. Keep your words very simple.", DateTimeCreated = DateTime.Now });
            }
            _db.Chats.Add(new Chat { SessionId = sessionId, Role = "user", Content = model.message, DateTimeCreated = DateTime.Now });
            _db.SaveChanges();

            chats = _db.Chats.Where(x => x.SessionId == sessionId).OrderBy(x => x.DateTimeCreated).ToList();

            var resp = new ChatGPTAPIModelResponseModel();
            using (var client = new HttpClient())
            {
                // clear the default headers to avoid issues
                client.DefaultRequestHeaders.Clear();

                // add header authorization and use your API KEY
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", APIKEY);


                ChatGPTAPIModelInput dalleInput = new ChatGPTAPIModelInput();
                dalleInput.model = "gpt-3.5-turbo";
                dalleInput.max_tokens = 2048;

                dalleInput.messages = new List<ChatGPTAPIModelInputMessage>();
                foreach (Chat chat in chats)
                {
                    ChatGPTAPIModelInputMessage msg = new ChatGPTAPIModelInputMessage();
                    msg.role = chat.Role;
                    msg.content = chat.Content;
                    dalleInput.messages.Add(msg);
                }

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


            ScenarioChatViewModel vm = new ScenarioChatViewModel();
            vm.message = resp.choices[0].message.content;
            vm.guid = sessionId;

            _db.Chats.Add(new Chat { SessionId = sessionId, Role = "assistant", Content = vm.message, DateTimeCreated = DateTime.Now });
            _db.SaveChanges();

            return Json(vm);
        }


        public IActionResult EduChatScenario()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EduChatScenarioPost([FromBody] ScenarioChatViewModel model)
        {
            string sessionId = model.guid;
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
            }
            IEnumerable<Chat> chats = _db.Chats.Where(x => x.SessionId == sessionId).ToList();
            if (chats.Count() == 0)
            {
                _db.Chats.Add(new Chat { SessionId = sessionId, Role = "system", Content = "Your name is Prajwal Deshkar. You are a friend but throughout the conversation, play close attention to the user's grammar and spelling. Correct it when needed or else just continue the conversation.", DateTimeCreated = DateTime.Now });
            }
            _db.Chats.Add(new Chat { SessionId = sessionId, Role = "user", Content = model.message, DateTimeCreated = DateTime.Now });
            _db.SaveChanges();

            chats = _db.Chats.Where(x => x.SessionId == sessionId).OrderBy(x => x.DateTimeCreated).ToList();

            var resp = new ChatGPTAPIModelResponseModel();
            using (var client = new HttpClient())
            {
                // clear the default headers to avoid issues
                client.DefaultRequestHeaders.Clear();

                // add header authorization and use your API KEY
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", APIKEY);


                ChatGPTAPIModelInput dalleInput = new ChatGPTAPIModelInput();
                dalleInput.model = "gpt-3.5-turbo";
                dalleInput.max_tokens = 2048;

                dalleInput.messages = new List<ChatGPTAPIModelInputMessage>();
                foreach (Chat chat in chats)
                {
                    ChatGPTAPIModelInputMessage msg = new ChatGPTAPIModelInputMessage();
                    msg.role = chat.Role;
                    msg.content = chat.Content;
                    dalleInput.messages.Add(msg);
                }

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


            ScenarioChatViewModel vm = new ScenarioChatViewModel();
            vm.message = resp.choices[0].message.content;
            vm.guid = sessionId;

            _db.Chats.Add(new Chat { SessionId = sessionId, Role = "assistant", Content = vm.message, DateTimeCreated = DateTime.Now });
            _db.SaveChanges();

            return Json(vm);
        }


        public IActionResult Test()
        {
            return View(_db.Chats.ToList());
        }

    }
}
