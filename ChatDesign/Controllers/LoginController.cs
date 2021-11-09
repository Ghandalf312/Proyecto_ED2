﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatDesign.Helpers;
using API.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace ChatDesign.Controllers
{
    public class LoginController : Controller
    {
        public LoginController()
        {
            Singleton.Instance().EnvironmentPath = Environment.CurrentDirectory;
            Singleton.Instance().APIClient = new HttpClient() { BaseAddress = new Uri("https://localhost:44335/api/") };
            Singleton.Instance().APIClient.DefaultRequestHeaders.Clear();
            Singleton.Instance().APIClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult LogOut()
        {
            HttpContext.Session.SetString("CurrentUser", string.Empty);
            HttpContext.Session.SetString("CurrentReceiver", string.Empty);
            return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult Login(IFormCollection collection)
        {
            try
            {
                var newUser = new User(collection["username"], collection["password"]);
                if (API.Models.User.CheckValidness(newUser))
                {
                    var users = GetUsers().Result.Where(user => user.Username == newUser.Username && user.Password == newUser.Password).ToList();
                    if (users.Count() != 0)
                    {
                        HttpContext.Session.SetString("CurrentUser", newUser.Username);
                        return RedirectToAction("Index", "Chatting");
                    }
                    else
                    {
                        ViewBag.Error = "Usuario o contraseña inválidos";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Error = "El usuario no existe, debe registrarse primero.";
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Register(IFormCollection collection)
        {
            try
            {
                if (collection["password"] != collection["confirmPassword"])
                {
                    ViewBag.Error = "Las contraseñas no coinciden";
                    return View();
                }
                var newUser = new User(collection["Username"], collection["Password"]);
                var users = GetUsers().Result.Where(user => user.Username == newUser.Username);
                if (users.Count() == 0)
                {
                    await Singleton.Instance().APIClient.PostAsJsonAsync("User", newUser);
                }
                else
                {
                    ViewBag.Error = "El usuario ya existe";
                    return View();
                }
                return RedirectToAction("Login");
            }
            catch
            {
                return View();
            }
        }
        private async Task<List<User>> GetUsers()
        {
            try
            {
                var users = new List<User>();
                var response = await Singleton.Instance().APIClient.GetAsync("User");
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
    }
}
