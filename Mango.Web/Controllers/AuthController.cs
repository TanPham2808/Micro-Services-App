﻿using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO loginRequestDTO = new();
            
            return View(loginRequestDTO);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin, Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer, Value=SD.RoleCustomer},
            };
            ViewBag.RoleList = roleList;

            return View();
        }

        [HttpGet]
        public IActionResult Assign()
        {
            RegisterationRequestDTO registerationRequestDTO = new();
            return View(registerationRequestDTO);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }
    }
}
