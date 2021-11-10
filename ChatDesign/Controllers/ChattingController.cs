using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using ChatDesign.Helpers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace ChatDesign.Controllers
{
    public class ChattingController : Controller
    {
        public ActionResult Index()
        {
            var listOfUsers = GetUsers().Where(user => user.Username != HttpContext.Session.GetString("CurrentUser"));
            return View(listOfUsers);
            
        }
        public ActionResult Chat(string receiver)
        {
            return View();
        }
        private List<User> GetUsers()
        {
            try
            {
                var users = new List<User>();
                var response = Singleton.Instance().APIClient.GetAsync("User").Result;
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<List<User>>(response.Content.ReadAsStringAsync().Result);
                }
                return new List<User>();
            }
            catch
            {
                return new List<User>();
            }
        }
        private List<Message> GetMessages(string currentUser, string receiver, bool isFile)
        {
            return new List<Message>();
        }
    }
}
