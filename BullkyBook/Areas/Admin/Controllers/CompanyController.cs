
using BullkyBook.DataAccess;
using BullkyBook.DataAccess.Repository.IRepository;
using BullkyBook.Models;
using BullkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using BullkyBook.Utility;

namespace BullkyBook.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

public class CompanyController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
      
    }

    public IActionResult Index()
    {
        return View();
    }
 

    //GET Method
    public IActionResult Upsert(int? id)
    {
        Company company = new();
      
        if (id == null || id == 0)
        {
            //create product   
            return View(company);
        }
        else
        {
            //update product
            company =_unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);

             return View(company);
        }

    }

    //POST Method
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(Company obj)
    {

        if (ModelState.IsValid)
        {
         
            if (obj.Id == 0)
            {
                _unitOfWork.Company.Add(obj);
                TempData["success"] = "Company created successfully";
            }
            else
            {
                _unitOfWork.Company.Update(obj);
                TempData["success"] = "Company updated successfully";
            }
            _unitOfWork.Save();         
            return RedirectToAction("Index");
        }
        return View(obj);
    }

    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var companyList = _unitOfWork.Company.GetAll();
        return Json(new { data = companyList });
    }
    //POST Method
    [HttpDelete]
    public IActionResult Delete(int? id)
    {

        var obj = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        _unitOfWork.Company.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion


}
