using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VgcCollege.MVC.Models;

namespace VgcCollege.MVC.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
}