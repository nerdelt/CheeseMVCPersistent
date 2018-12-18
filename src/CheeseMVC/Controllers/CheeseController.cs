﻿using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Models;
using System.Collections.Generic;
using CheeseMVC.ViewModels;
using CheeseMVC.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CheeseMVC.Controllers
{
    public class CheeseController : Controller
    {
        private readonly CheeseDbContext context;

        public CheeseController(CheeseDbContext dbContext)
        {
            this.context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            IList<Cheese> cheeses = context.Cheeses.Include(c => c.Category).ToList();
            return View(cheeses);
        }

        public IActionResult Add()
        {
            AddCheeseViewModel addCheeseViewModel = 
                new AddCheeseViewModel(context.Categories.ToList());
            return View(addCheeseViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddCheeseViewModel addCheeseViewModel)
        {
            if (ModelState.IsValid)
            {
                CheeseCategory newCheeseCategory = 
                    context.Categories.Single(c => c.ID == addCheeseViewModel.CategoryID);
                // Add the new cheese to my existing cheeses
                Cheese newCheese = new Cheese
                {
                    Name = addCheeseViewModel.Name,
                    Description = addCheeseViewModel.Description,
                    Category = newCheeseCategory
                };

                context.Cheeses.Add(newCheese);
                context.SaveChanges();

                return Redirect("/Cheese");
            }

            return View(addCheeseViewModel);
        }

        public IActionResult Remove()
        {
            ViewBag.title = "Remove Cheeses";
            ViewBag.cheeses = context.Cheeses.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Remove(int[] cheeseIds)
        {
            foreach (int cheeseId in cheeseIds)
            {
                Cheese theCheese = context.Cheeses.Single(c => c.ID == cheeseId);
                context.Cheeses.Remove(theCheese);
            }

            context.SaveChanges();

            return Redirect("/");
        }

        public IActionResult Category(int id)
        {
            if (id == 0)
            {
                return Redirect("/Category");
            }

            CheeseCategory theCategory = context.Categories
                .Include(cat => cat.Cheeses)
                .Single(cat => cat.ID == id);

            ViewBag.title = "Cheese in category: " + theCategory.Name;

            return View("Index" , theCategory.Cheeses);
        }

        public IActionResult Edit(int cheeseId)
        { 
            Cheese toEdit = context.Cheeses.Single(c => c.ID == cheeseId);

            EditCheeseViewModel editCheeseViewModel =
                new EditCheeseViewModel(context.Categories.ToList());


            editCheeseViewModel.Name = toEdit.Name;
            editCheeseViewModel.Description = toEdit.Description;
            editCheeseViewModel.CategoryID = toEdit.CategoryID;
            editCheeseViewModel.ID = cheeseId;
               
               
            return View(editCheeseViewModel);
        }

        
        [HttpPost]
        public IActionResult Edit(Cheese toEdit, int cheeseId, EditCheeseViewModel editCheeseViewModel)
        {
            if (ModelState.IsValid)
            {
                context.Update(toEdit);
                context.SaveChanges();
            }

                return Redirect("/");
        }
    }
}
