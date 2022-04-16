﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TMSClient.Models;

namespace TMSClient.Controllers
{
    public class TrainerManagersController : Controller
    {
        IConfiguration _configuration;
        string BaseURL;
        public TrainerManagersController(IConfiguration configuration)
        {

            _configuration = configuration;
            BaseURL = _configuration.GetValue<string>("BaseURL");
        }
        public async Task<ActionResult> Index()
        {
            List<TrainerManager> trainerManagers = await GetTrainerManagers();
            return View(trainerManagers);
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TrainerManager trainerManager)
        {
            try
            {

                TrainerManager recievedTM = new TrainerManager();
                HttpClientHandler clientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(clientHandler);
                StringContent content = new StringContent(JsonConvert.SerializeObject(trainerManager), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(BaseURL + "/api/TrainerManagers", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    recievedTM = JsonConvert.DeserializeObject<TrainerManager>(apiResponse);
                    if (recievedTM != null)
                    {
                        return RedirectToAction("DashBoard", "trainermanagers");
                    }
                }


                ViewBag.Message = "Your Record not Created!!! Please try again";
                return View();

            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

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

        public ActionResult Delete(int id)
        {
            return View();
        }

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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(TrainerManager tm)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Inputs are not valid";
                return View();
            }
            List<TrainerManager> admins = await GetTrainerManagers();
            var obj = admins.Where(a => a.EmailID.Equals(tm.EmailID) && a.Password.Equals(tm.Password)).FirstOrDefault();
            if (obj != null)
            {
                HttpContext.Session.SetString("EmailID", obj.EmailID.ToString());
                HttpContext.Session.SetString("Role", obj.Role.ToString());
                return RedirectToAction("DashBoard", "TrainerManagers");
            }
            else
            {
                ViewBag.Message = "User not found for given Email and Password";
                return View();
            }
        }


        public IActionResult DashBoard()
        {
            return View();
        }

        public async Task<List<TrainerManager>> GetTrainerManagers()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/trainermanagers");
            var result = JsonConvert.DeserializeObject<List<TrainerManager>>(JsonStr);
            return result;
        }
    }
}
