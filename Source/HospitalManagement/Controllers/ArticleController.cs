using BELibrary.Core.Entity;
using BELibrary.DbContext;
using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Controllers
{
    public class ArticleController : Controller
    {
        // GET: Article
        public ActionResult Index(string query, int? page)
        {
            if (query == "")
            {
                query = null;
            }

            ViewBag.QueryData = query;
            var pageNumber = (page ?? 1);
            const int pageSize = 5;

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var listData = workScope.Articles.Query(x => !x.IsDelete);

                if (!string.IsNullOrEmpty(query))
                    listData = listData.Where(x => x.Title.ToLower().Contains(query.Trim().ToLower())
                                                   || !string.IsNullOrEmpty(x.Description) && x.Description.ToLower().Contains(query.Trim().ToLower())
                                                   || !string.IsNullOrEmpty(x.Content) && x.Content.ToLower().Contains(query.Trim().ToLower())

                    );

                var arts = listData.ToList();
                ViewBag.Total = arts.Count();

                var latestPosts = workScope.Articles.Query(x => !x.IsDelete).OrderByDescending(x => x.ModifiedDate).Take(5).ToList();
                ViewBag.LatestPosts = latestPosts;

                return View(arts.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult Detail(Guid id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var latestPosts = workScope.Articles.Query(x => !x.IsDelete).OrderByDescending(x => x.ModifiedDate).Take(5).ToList();
                ViewBag.LatestPosts = latestPosts;

                var article = workScope.Articles.FirstOrDefault(x => x.Id == id && !x.IsDelete);
                if (article != null)
                {
                    return View(article);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }
    }
}