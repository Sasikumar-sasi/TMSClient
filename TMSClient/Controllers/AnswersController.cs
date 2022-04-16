﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TMSClient.Models;
using TMSClient.Models.ViewModel;

namespace TMSClient.Controllers
{
    public class AnswersController : Controller
    {
        IConfiguration _configuration;
        string BaseURL;
        IHostEnvironment _environment;

        public AnswersController(IConfiguration configuration, IHostEnvironment environment)
        {
            _environment = environment;
            _configuration = configuration;
            BaseURL = _configuration.GetValue<string>("BaseURL");
        }
        // GET: AnswersController
        public async Task<ActionResult> Index(int id)
        {
            List<Answer> answerList = await GetAllAnswers();
            List<Answer> answerByID = answerList.Where(ans=>ans.AssessmentID==id).ToList();
            return View(answerByID);
        }

        // GET: AnswersController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AnswersController/Create
        public ActionResult Create(int id)
        {
            ViewBag.AssessmentID = id;
            ViewBag.ID = Convert.ToInt32(HttpContext.Session.GetString("ID"));
            return View();
        }

        // POST: AnswersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AnswerViewModel answerViewModel)
        {
            try
            {
                Console.WriteLine(answerViewModel.AnswerFile.FileName);
                string UniqueFileName = null;
                if (answerViewModel.AnswerFile != null)
                {
                    Console.WriteLine("Checking folder");
                    string UploadFolder = "QuestionsAndAnswers/Answers";
                    Console.WriteLine("Folder is correct");
                    UniqueFileName = Guid.NewGuid().ToString() + "-" + answerViewModel.AnswerFile.FileName;
                    string FilePath = Path.Combine(UploadFolder, UniqueFileName + Path.GetExtension(answerViewModel.AnswerFile.FileName));
                    answerViewModel.AnswerFile.CopyTo(new FileStream(FilePath, FileMode.Create));

                    Answer answer= new Answer()
                    {
                        AssessmentID = answerViewModel.AssessmentID,
                        TraineeID =Convert.ToInt32(HttpContext.Session.GetString("ID")),
                        AnswerPath = UniqueFileName

                    };

                    Answer recievedAnswer = new Answer();
                    HttpClientHandler clientHandler = new HttpClientHandler();
                    var httpClient = new HttpClient(clientHandler);
                    StringContent content = new StringContent(JsonConvert.SerializeObject(answer), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync(BaseURL + "/api/Answers", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        recievedAnswer = JsonConvert.DeserializeObject<Answer>(apiResponse);
                        if (recievedAnswer != null)
                        {
                            return RedirectToAction("DashBoard", "Trainees");
                        }
                        else
                        {
                            Console.WriteLine("Server issue");
                            return View();
                        }
                    }
                    
                }
                else
                {
                    Console.WriteLine("FileName is Empty");
                    return View();
                }

            }
            catch
            {
                Console.WriteLine("Catch Bloack executed");
                return View();
            }
        }

        public async Task<IActionResult> DownloadAnswer(string fileName)
        {

            if (fileName == null)
                return Content("filename not present");
            string QuestionFolder = "QuestionsAndAnswers/Answers";

            var path = Path.Combine(QuestionFolder, fileName + Path.GetExtension(fileName));

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            var contentType = "APPLICATION/octet-stream";
            return File(memory, contentType, Path.GetFileName(path));
        }

        // GET: AnswersController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AnswersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AnswersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AnswersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        public async Task<List<Answer>> GetAllAnswers()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/answers");
            var result = JsonConvert.DeserializeObject<List<Answer>>(JsonStr);
            return result;
        }
    }
}
