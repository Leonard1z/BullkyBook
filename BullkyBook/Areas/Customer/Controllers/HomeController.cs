﻿using BullkyBook.DataAccess.Repository.IRepository;
using BullkyBook.Models;
using BullkyBook.Models.ViewModels;
using BullkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BullkyBook.Controllers;
[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");

        return View(productList);
    }

    public IActionResult Details(int productId)
    {
        ShoppingCart cartObj = new()
        {
            Count = 1,
             Product = _unitOfWork.Product.GetFirstOrDefault(u => u.ID == productId, includeProperties: "Category,CoverType"),
        };

        return View(cartObj);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //Cant Aceess without login(Authorize)
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {

        var claimsidentity = (ClaimsIdentity)User.Identity;
        var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);
        shoppingCart.ApplicationUserId = claim.Value;


        ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
            u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);

        if (cartFromDb == null)
        {
            _unitOfWork.ShoppingCart.Add(shoppingCart);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
        }
        else
        {
            _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
            _unitOfWork.Save();
        }

        
        return RedirectToAction(nameof(Index));
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
