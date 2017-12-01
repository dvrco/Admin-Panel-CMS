using ShopingCardCMS.Models.Data;
using ShopingCardCMS.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopingCardCMS.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // Declare list of PageVM
            List<PageVM> pagesList;
            

            using(Db db = new Db())
            {
                //Init the list
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();

            }

            // Return view with list
            return View(pagesList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            // check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Models.Data.Db())
            {


                // declare slug
                string slug;
                // init pageDTO
                PageDTO dto = new PageDTO();
                // DTO title
                dto.Title = model.Title;
                // check for and set slug if need be
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                // make sure title and slug are unique
                if(db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That title or slug already exists.");
                    return View(model);
                }
                // DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                // save DTO
                db.Pages.Add(dto);
                db.SaveChanges();
            }
            // Set TempData message
            TempData["SM"] = "You have added a new page!";
            // Redirect
            return RedirectToAction("AddPage");
        }

        [HttpGet]
        public ActionResult EditPage(int id)
        {
            // declare pageVM
            PageVM model;

            using(Db db = new Db())
            {
                PageDTO dto = db.Pages.Find(id);

                if (dto == null)
                {
                    return Content("The page does not exist.");
                }

                model = new PageVM(dto);             
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                int id = model.Id;
                string slug = "home";
                PageDTO dto = db.Pages.Find(id);
                dto.Title = model.Title;
                if(model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                if(db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) || 
                    db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That title or slug already exists.");
                    return View(model);
                }

                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                db.SaveChanges();

                TempData["SM"] = "You have edited the page";

                return RedirectToAction("EditPage");
            }
            return View();
        }

        public ActionResult PageDetails(int id) 
        {
            PageVM model;

            using (Db db = new Db())
            {
                PageDTO dto = db.Pages.Find(id);
                if(dto == null)
                {
                    return Content("The page does not exist.");
                }

                model = new PageVM(dto);
            }
            return View(model);
        }

        public ActionResult DeletePage(int id)
        {

            using (Db db = new Db())
            {
                PageDTO dto = db.Pages.Find(id);
                db.Pages.Remove(dto);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using(Db db = new Db())
            {
                int count = 1;
                PageDTO dto;
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }
            
        }

        [HttpGet]
        public ActionResult EditSidebar()
        {
            SidebarVM model;
            using(Db db = new Db())
            {
                SidebarDTO dto = db.Sidebar.Find(1);

                model = new SidebarVM(dto);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EidtSidebar(SidebarVM model)
        {
            using(Db db = new Db())
            {
                SidebarDTO dto = db.Sidebar.Find(1);
                dto.Body = model.Body;
                db.SaveChanges();
            }

            TempData["SM"] = "You have edited the sidebar!";

            return RedirectToAction("EditSidebar");
        }

        // dlt
        [HttpPost]
        public ActionResult EditTemplate(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                int id = model.Id;
                string slug = "home";
                PageDTO dto = db.Pages.Find(id);
                dto.Title = model.Title;
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) ||
                    db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That title or slug already exists.");
                    return View(model);
                }

                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                db.SaveChanges();

                TempData["SM"] = "You have edited the page";

                return RedirectToAction("EditPage");
            }
            return View();
        }
    }
}