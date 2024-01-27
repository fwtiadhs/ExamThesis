﻿using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamThesis.Controllers
{
    public class QuestionCategoryController : Controller
    {
        private readonly ExamContext _db;
        public QuestionCategoryController(ExamContext db)
        {
            _db= db;
        }

        public IActionResult Index()
        {
            IEnumerable<QuestionCategory> objQuestionCategoryList = _db.QuestionCategories.ToList();
            ViewBag.QuestionCategoryList = _db.QuestionCategories.ToList();

            return View() ;
        }

        [HttpGet]
        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamThesis.Models.QuestionCategory obj)
        {
            if (ModelState.IsValid)
            {
                var model = new ExamThesis.Storage.Model.QuestionCategory()
                {
                    QuestionCategoryName = obj.QuestionCategoryName,

                };
                _db.QuestionCategories.Add(model);
                await _db.SaveChangesAsync();
                ViewBag.QuestionCategoryList = _db.QuestionCategories.ToList();

            }
            return View("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
           
            var QCategoryFromDbFirst = _db.QuestionCategories.FirstOrDefault(u => u.QuestionCategoryId == id);

            if (QCategoryFromDbFirst == null)
            {
                return NotFound();
            }

            return View(QCategoryFromDbFirst);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(QuestionCategory obj)
        {
          

            if (ModelState.IsValid) { 

                _db.QuestionCategories.Update(obj);
                _db.SaveChanges();

             return RedirectToAction("Index");
            }
            return View(obj);
        }


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var QCategoryFromDb = _db.QuestionCategories.Find(id);
         

            if (QCategoryFromDb == null)
            {
                return NotFound();
            }

            return View(QCategoryFromDb);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _db.QuestionCategories.Find(id);

            if (obj == null)
            {
                return NotFound();
            }

            _db.QuestionCategories.Remove(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
        }

    }
}
