using BobaTeaFace.Data;
using BobaTeaFace.Data.Migrations;
using BobaTeaFace.Models;
using BobaTeaFace.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using static Humanizer.In;

namespace BobaTeaFace.Controllers
{
    public class ModulesController : Controller
    {
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        private string APIKEY = string.Empty;
        private ApplicationDbContext _db;

        public ModulesController(IConfiguration conf, ApplicationDbContext db, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _db = db;
            APIKEY = conf.GetSection("OPENAI_API_KEY").Value;
            _hostingEnvironment = environment;
        }

        public IActionResult Index()
        {
            IEnumerable<Course> courses = _db.Courses.ToList();
            return View(courses);
        }
        public IActionResult GameMatchMe()
        {
            return Redirect("http://gamematchme.com");
            //return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Course course)
        {

            _db.Courses.Add(course);
            _db.SaveChanges();

            return RedirectToAction("");
        }

        public IActionResult Details(int Id)
        {
            Course c = _db.Courses.Where(x => x.Id == Id).FirstOrDefault();
            return View(c);
        }

        public IActionResult Edit(int Id)
        {
            Course c = _db.Courses.Where(x => x.Id == Id).FirstOrDefault();
            return View(c);
        }
        [HttpPost]
        public IActionResult Edit(Course course)
        {
            Course oldCourse= _db.Courses.Where(x => x.Id == course.Id).FirstOrDefault();
            oldCourse.Title = course.Title;
            oldCourse.Description = course.Description; 
            oldCourse.ChildImageUrl = course.ChildImageUrl;

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
                    file.CopyTo(fileStream);
                }
            }

            oldCourse.ExternalImageUrl = course.ExternalImageUrl;
            oldCourse.Gender = course.Gender;

            oldCourse.WhatQ = course.WhatQ;
            oldCourse.WhatA = course.WhatA;

            oldCourse.WhyQ = course.WhyQ;
            oldCourse.WhyA = course.WhyA;

            oldCourse.WhoQ = course.WhoQ;
            oldCourse.WhoA = course.WhoA;

            oldCourse.HowQ = course.HowQ;
            oldCourse.HowA= course.HowA;

            oldCourse.WhenQ = course.WhenQ;
            oldCourse.WhenA = course.WhenA;

            oldCourse.WhereQ = course.WhereQ;
            oldCourse.WhereA = course.WhereA;
            

            _db.Entry(oldCourse).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _db.SaveChanges();

            return RedirectToAction("Edit", new {Id = oldCourse.Id});
        }

        [HttpPost]
        public async Task<IActionResult> GenerateScenarioBasedImage()
        {
            string childImageUrl = "";
            if (Request.Form.Where(x => x.Key.ToLower() == "childImageUrl").Count() > 0)
            {
                KeyValuePair<string, StringValues> formScenario = Request.Form.Where(x => x.Key == "childImageUrl").FirstOrDefault();
                childImageUrl = formScenario.Value;
            } 

            string gender = "boy";
            if (Request.Form.Where(x => x.Key.ToLower() == "gender").Count() > 0)
            {
                KeyValuePair<string, StringValues> formGender = Request.Form.Where(x => x.Key == "gender").FirstOrDefault();
                if (formGender.Value == "20")
                {
                    gender = "girl";
                }
            }

            string scenarioTitle = "";
            if (Request.Form.Where(x => x.Key.ToLower() == "title").Count() > 0)
            {
                KeyValuePair<string, StringValues> formScenario = Request.Form.Where(x => x.Key == "title").FirstOrDefault();
                scenarioTitle = formScenario.Value;
            }

            string scenario = "";
            if (Request.Form.Where(x => x.Key.ToLower() == "scenario").Count() > 0)
            {
                KeyValuePair<string, StringValues> formScenario = Request.Form.Where(x => x.Key == "scenario").FirstOrDefault();
                scenario = formScenario.Value;
            }

            string fileUrl = "https://bobateaapp.theaisyaaziz.com/uploads/";
            string externalfileUrl = "https://bobateaapp.theaisyaaziz.com/uploads/";
            IFormFileCollection files = Request.Form.Files;
            if (files.Count() > 0)
            {
                IFormFile file = files.FirstOrDefault();
                string uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                string filePath = Path.Combine(uploads, file.FileName);
                fileUrl = fileUrl + file.FileName;
                try
                {
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                } catch (Exception ex)
                {

                }
            } else
            {
                if (!(string.IsNullOrEmpty(childImageUrl)))
                {
                    fileUrl = childImageUrl;
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
                dalleInput.model = "image-alpha-001";
                dalleInput.n = 1;
                dalleInput.size = "256x256";
                dalleInput.prompt = fileUrl + "***" + "A " + gender + " doing: " + scenario + ", but as a photography";

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
                    var imageUrl = dallEResponse.data.FirstOrDefault().url;

                    string fileName = Guid.NewGuid() + ".jpg";
                    string uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                    string filePath = Path.Combine(uploads, fileName);
                    externalfileUrl = externalfileUrl + fileName;

                    using (var client2 = new HttpClient())
                    {
                        var imageBytes = await client2.GetByteArrayAsync(imageUrl);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                        }
                    }

                }
            }

            ScenarioDallEResponseViewModel vm = new ScenarioDallEResponseViewModel();
            vm.childImageUrl = fileUrl;
            vm.scenario = scenario;
            vm.externalImageUrl = externalfileUrl;
            //if (dallEResponse != null)
            //{
            //    vm.data = dallEResponse.data;
            //}
            return Json(vm);
        }


        [HttpPost]
        public async Task<IActionResult> GenerateQnA([FromBody] ScenarioWHRequestViewModel model)
        {

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

                string tempmessage = @"Generate 5W and 1 H questions based on the scenario: A " + gender + " doing: " + model.Scenario  + ".";
                tempmessage = tempmessage + "Remember it is for Autistic children so keep English simple with limited vocabulary, also keep images simple and happy with minimal distractions.";
                tempmessage = tempmessage + @"The output should be in the format below as I am feeding it to code so just answer as below no extra words:
                5W and 1H Questions:
                Who Question: 
                What Question: 
                Where Question:
                When Question: 
                Why Question:
                How Question: 

                5W and 1H Answers:
                Who Answer: 
                What Answer: 
                Where Answer:
                When Answer: 
                Why Answer:
                How Answer:";


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

            ScenarioWHResponseViewModel vm = new ScenarioWHResponseViewModel();
            foreach (var theline in message.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (theline.StartsWith("Who Question:"))
                {
                    vm.WhoQ = theline.Substring(theline.IndexOf(":") + 1);
                }
                else if (theline.StartsWith("What Question:"))
                {
                    vm.WhatQ = theline.Substring(theline.IndexOf(":") + 1);
                }
                else if (theline.StartsWith("Where Question:"))
                {
                    vm.WhereQ = theline.Substring(theline.IndexOf(":") + 1);
                }
                else if (theline.StartsWith("When Question:"))
                {
                    vm.WhenQ = theline.Substring(theline.IndexOf(":") + 1);
                }
                else if (theline.StartsWith("Why Question:"))
                {
                    vm.WhyQ = theline.Substring(theline.IndexOf(":") + 1);
                }
                else if (theline.StartsWith("How Question:"))
                {
                    vm.HowQ = theline.Substring(theline.IndexOf(":") + 1);
                }
                else if (theline.StartsWith("Who Answer:"))
                {
                    vm.WhoA = theline.Substring(theline.IndexOf(":") + 1);
                }
                else if (theline.StartsWith("What Answer:"))
                {
                    vm.WhatA = theline.Substring(theline.IndexOf(":") + 1);
                }
                else if (theline.StartsWith("Where Answer:"))
                {
                    vm.WhereA = theline.Substring(theline.IndexOf(":") + 1);
                }
                else if (theline.StartsWith("When Answer:"))
                {
                    vm.WhenA = theline.Substring(theline.IndexOf(":") + 1);
                }
                else if (theline.StartsWith("Why Answer:"))
                {
                    vm.WhyA = theline.Substring(theline.IndexOf(":") + 1);
                }
                else if (theline.StartsWith("How Answer:"))
                {
                    vm.HowA = theline.Substring(theline.IndexOf(":") + 1);
                }
            }

        
            return Json(vm);
        }


    }
}
