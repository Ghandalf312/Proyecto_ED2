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
using CiphersAndCompression.Compressor;
using System.IO;

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
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            var messages = GetMessages(HttpContext.Session.GetString("CurrentUser"), receiver, false);
            var files = GetMessages(HttpContext.Session.GetString("CurrentUser"), receiver, true);
            var conversation = new Conversation(messages, files, currentUser, receiver);
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
                    Text = cipheredMessage,
                    OnlySender = false
                };
                await Singleton.Instance().APIClient.PostAsJsonAsync("Chat", messageForUpload);
                return RedirectToAction("Chat");
            }
            catch
            {
                return RedirectToAction("Chat");
            }
        }
        [HttpPost]
        public async Task<ActionResult> UploadFileAsync(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    ViewBag.ErrorMessage = "Seleccione un archivo antes de enviar.";
                    return RedirectToAction("Chat");
                }
                var currentUser = HttpContext.Session.GetString("CurrentUser");
                var receiver = HttpContext.Session.GetString("CurrentReceiver");
                var savedFileRoute = await FileManager.SaveFileAsync(file, Singleton.Instance().EnvironmentPath, false);
                var compressor = new LZW();
                var compressedFilePath = compressor.CompressFile(Singleton.Instance().EnvironmentPath, savedFileRoute, Path.GetFileName(savedFileRoute));
                var fileStream = System.IO.File.OpenRead(compressedFilePath);
                var multiForm = new MultipartFormDataContent
                {
                    { new StreamContent(fileStream), "file", Path.GetFileName(compressedFilePath) }
                };
                var response = await Singleton.Instance().APIClient.PostAsync("File", multiForm);
                var fileNameInAPI = await response.Content.ReadAsStringAsync();
                fileNameInAPI = fileNameInAPI.Remove(0, 1);
                fileNameInAPI = fileNameInAPI.Remove(fileNameInAPI.Length - 1, 1);
                var SDESKey = SDES.GetSecretKey(GetUserSecretNumber(currentUser), GetUserPublicKey(receiver));
                var cipher = new SDES();
                var cipheredMessage = cipher.EncryptString(fileNameInAPI, SDESKey);
                var pathMessage = new Message() { Text = cipheredMessage, IsFile = true, Sender = currentUser, Receiver = receiver, OnlySender = false };
                await Singleton.Instance().APIClient.PostAsJsonAsync("Chat", pathMessage);
                return RedirectToAction("Chat");
            }
            catch
            {
                return RedirectToAction("Chat");
            }
        }

        [Route("DownloadFile")]
        public async Task<ActionResult> DownloadFileAsync(string message)
        {
            try
            {
                var currentUser = HttpContext.Session.GetString("CurrentUser");
                var receiver = HttpContext.Session.GetString("CurrentReceiver");
                var newMessage = new Message() { Text = message };
                var result = await Singleton.Instance().APIClient.PostAsJsonAsync("File/GetFile", newMessage);
                var fileForDownloading = await result.Content.ReadAsStreamAsync();
                var route = await FileManager.SaveDownloadedStream(fileForDownloading, Singleton.Instance().EnvironmentPath, newMessage.Text);
                var decompressor = new LZW();
                var decompressedFilePath = decompressor.DecompressFile(Singleton.Instance().EnvironmentPath, route, message);
                decompressedFilePath = Path.GetFullPath(decompressedFilePath);
                var fileArray = System.IO.File.ReadAllBytes(decompressedFilePath);
                return File(fileArray, "text/plain", message);
            }
            catch
            {
                return RedirectToAction("Chat");
            }
        }

        public ActionResult SearchMessages(string receiver, string searchedValue)
        {
            if (receiver != null)
            {
                HttpContext.Session.SetString("CurrentReceiver", receiver);
            }
            else
            {
                receiver = HttpContext.Session.GetString("CurrentReceiver");
            }
            var searchedMessages = GetMessages(HttpContext.Session.GetString("CurrentUser"), receiver, false);
            searchedMessages = searchedMessages.Where(message => message.Text.Contains(searchedValue)).ToList();
            var conversation = new Conversation(searchedMessages, receiver, searchedValue);
            return View(conversation);
        }

        [HttpPost]
        public ActionResult SearchMessages(IFormCollection collection)
        {
            try
            {
                var searched = collection["SearchedValue"];
                if (searched == string.Empty)
                {
                    ViewBag.ErrorMessage = "Coloque el parámetro a buscar";
                    return RedirectToAction("Chat");
                }
                var receiverUser = HttpContext.Session.GetString("CurrentReceiver");
                return RedirectToAction("SearchMessages", new { receiver = receiverUser, searchedValue = searched });
            }
            catch
            {
                return RedirectToAction("Chat");
            }
        }

        public ActionResult DeleteMessage(string messageToDelete, string receiver)
        {
            try
            {
                var sender = HttpContext.Session.GetString("CurrentUser");
                var messages = GetMessages(sender, receiver, false);
                var SDESKey = SDES.GetSecretKey(GetUserSecretNumber(sender), GetUserPublicKey(receiver));
                var cipher = new SDES();
                foreach (var message in messages)
                {
                    if (messageToDelete == message.Text)
                    {
                        var messageForUpload = new Message()
                        {
                            Id = message.Id,
                            Receiver = receiver,
                            IsFile = false,
                            Sender = sender,
                            Text = cipher.EncryptString(message.Text, SDESKey),
                            OnlySender = true
                        };
                        Singleton.Instance().APIClient.PutAsJsonAsync("Chat", messageForUpload);
                    }
                }
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
