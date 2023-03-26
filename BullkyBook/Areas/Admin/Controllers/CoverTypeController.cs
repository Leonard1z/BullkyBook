
using BullkyBook.DataAccess;
using BullkyBook.DataAccess.Repository.IRepository;
using BullkyBook.Models;
using BullkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BullkyBook.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
public class CoverTypeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CoverTypeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<CoverType> objCoverTypeList = _unitOfWork.CoverType.GetAll();
        return View(objCoverTypeList);
    }
    //GET Method
    public IActionResult Create()
    {

        return View();
    }

    //POST Method
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CoverType obj)
    {
     
        //Nese eshte null ktheje view
        if (ModelState.IsValid)
        {
            _unitOfWork.CoverType.Add(obj);
            _unitOfWork.Save();
            TempData["success"] = "CoverType created successfully";
            return RedirectToAction("Index");
        }
        return View(obj);
    }

    //GET Method
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var coverTypeFromDbFirst = _unitOfWork.CoverType.GetFirstOrDefault(u=>u.Id == id);
   

        if (coverTypeFromDbFirst == null)
        {
            return NotFound();
        }
        return View(coverTypeFromDbFirst);
    }

    //POST Method
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(CoverType obj)
    {
      
        //Nese eshte null ktheje view
        if (ModelState.IsValid)
        {
            _unitOfWork.CoverType.Update(obj);
            _unitOfWork.Save();
            TempData["success"] = "CoverType updated successfully";
            return RedirectToAction("Index");
        }
        return View(obj);
    }

    //GET Method
    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
       
        var coverTypeFromDbFirst = _unitOfWork.CoverType.GetFirstOrDefault(u=>u.Id == id);
        

        if (coverTypeFromDbFirst == null)
        {
            return NotFound();
        }
        return View(coverTypeFromDbFirst);
    }

    //POST Method
    [HttpPost,ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePOST(int? id)
    {
    
        var obj = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return NotFound();
        }
        _unitOfWork.CoverType.Remove(obj);
        _unitOfWork.Save();
             TempData["success"] = "CoverType deleted successfully";
        return RedirectToAction("Index");
        
    }
}
