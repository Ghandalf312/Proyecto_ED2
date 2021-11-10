using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using ChatDesign.Helpers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using ChatDesign.Models;
using CiphersAndCompression.Ciphers;
using System.Net.Http;

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
            if (receiver != null)
            {
                HttpContext.Session.SetString("CurrentReceiver", receiver);
            }
            else
            {
                receiver = HttpContext.Session.GetString("CurrentReceiver");
            }
            var messages = GetMessages(HttpContext.Session.GetString("CurrentUser"), receiver, false);
            var files = GetMessages(HttpContext.Session.GetString("CurrentUser"), receiver, true);
            var conversation = new Conversation(messages, files, receiver);
            return View(conversation);
        }
        [HttpPost]
        public async Task<ActionResult> Chat(IFormCollection collection)
        {
            try
            {
                var message = collection["Message"];
                if (message == string.Empty)
                {
                    ViewBag.ErrorMessage = "Coloque el parámetro a buscar";
                    return RedirectToAction("Chat");
                }
                var currentUser = HttpContext.Session.GetString("CurrentUser");
                var receiver = HttpContext.Session.GetString("CurrentReceiver");
                var SDESKey = SDES.GetSecretKey(GetUserSecretNumber(currentUser), GetUserPublicKey(receiver));
                var cipher = new SDES();
                var cipheredMessage = cipher.EncryptString(message, SDESKey);
                var messageForUpload = new Message()
                {
                    Receiver = receiver,
                    IsFile = false,
                    Sender = currentUser,
                    Text = cipheredMessage
                };
                await Singleton.Instance().APIClient.PostAsJsonAsync("Chat", messageForUpload);
                return RedirectToAction("Chat");
            }
            catch
            {
                return RedirectToAction("Chat");
            }
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
        private int GetUserPublicKey(string username)
        {
            try
            {
                var user = GetUsers().FirstOrDefault(user => user.Username == username);
                return user.PublicKey;
            }
            catch
            {
                return 0;
            }
        }
        private int GetUserSecretNumber(string username)
        {
            try
            {
                var user = GetUsers().FirstOrDefault(user => user.Username == username);
                return user.SecretNumber;
            }
            catch
            {
                return 0;
            }
        }
        private List<Message> GetMessages(string currentUser, string receiver, bool isFile)
        {
            try
            {
                var messages = new List<Message>();
                var response = Singleton.Instance().APIClient.GetAsync("Chat").Result;
                if (response.IsSuccessStatusCode)
                {
                    var messageList = JsonConvert.DeserializeObject<List<Message>>(response.Content.ReadAsStringAsync().Result);
                    var conversationMessages = new List<Message>();
                    if (isFile)
                    {
                        conversationMessages = messageList.Where(m => ((m.Sender == currentUser && m.Receiver == receiver) || (m.Sender == receiver && m.Receiver == currentUser)) && m.IsFile).ToList();
                    }
                    else
                    {
                        conversationMessages = messageList.Where(m => ((m.Sender == currentUser && m.Receiver == receiver) || (m.Sender == receiver && m.Receiver == currentUser)) && !m.IsFile).ToList();
                    }

                    if (conversationMessages.Count != 0 && !isFile)
                    {
                        var SDESKey = SDES.GetSecretKey(GetUserSecretNumber(currentUser), GetUserPublicKey(receiver));
                        var cipher = new SDES();
                        foreach (var message in conversationMessages)
                        {
                            message.Text = cipher.DecryptString(message.Text, SDESKey);
                        }
                        return conversationMessages;
                    }
                    else if (conversationMessages.Count != 0)
                    {
                        var SDESKey = SDES.GetSecretKey(GetUserSecretNumber(currentUser), GetUserPublicKey(receiver));
                        var cipher = new SDES();
                        foreach (var message in conversationMessages)
                        {
                            message.Text = cipher.DecryptString(message.Text, SDESKey);
                        }
                        return conversationMessages;
                    }
                }
                return new List<Message>();
            }
            catch
            {
                return new List<Message>();
            }
        }
    }
}
