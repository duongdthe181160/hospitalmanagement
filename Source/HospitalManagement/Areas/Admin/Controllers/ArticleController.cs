using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using BELibrary.Utils;
using HospitalManagement.Areas.Admin.Authorization;
using PagedList;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class ArticleController : BaseController
    {
        private const string KeyElement = "Bài viết";

        // GET: Admin/Article
        public ActionResult Index()
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = KeyElement;
            return RedirectToAction("Search");
        }

        public ActionResult Search(string query, int? page)
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = KeyElement;
            ViewBag.Host = (Request.Url == null ? "" : Request.Url.Host);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // the code that you want to measure comes here

            if (query == "")
            {
                query = null;
            }

            ViewBag.QueryData = query;
            var pageNumber = (page ?? 1);
            const int pageSize = 5;

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var listData = workScope.Articles.Query(x => !x.IsDelete).OrderByDescending(x => x.ModifiedDate).ToList();

                double elapsedMs = 0;
                if (query == null)
                {
                    ViewBag.Total = listData.Count();
                    watch.Stop();

                    elapsedMs = (double)watch.ElapsedMilliseconds / 1000;
                    ViewBag.RequestTime = elapsedMs;
                    return View(listData.ToPagedList(pageNumber, pageSize));
                }

                var q = (from mt in listData
                         where (!string.IsNullOrEmpty(query) &&
                                (mt.Title.ToLower().Contains(query.ToLower())
                                 || !string.IsNullOrEmpty(mt.Description) && mt.Description.ToLower().Contains(query.ToLower())
                                 || !string.IsNullOrEmpty(mt.Content) && mt.Content.ToLower().Contains(query.ToLower())))

                         select mt).AsQueryable();

                ViewBag.Total = q.Count();
                watch.Stop();

                elapsedMs = (double)watch.ElapsedMilliseconds / 1000;
                ViewBag.RequestTime = elapsedMs;
                return View(q.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult Detail(Guid id)
        {
            ViewBag.Element = KeyElement;
            ViewBag.Feature = "Chi tiết";
            ViewBag.Element = KeyElement;
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var regimen = workScope.Articles.FirstOrDefault(x => x.Id == id);
                if (regimen != null)
                {
                    return View(regimen);
                }
                else
                {
                    return RedirectToAction("Create", "Article");
                }
            }
        }

        [Permission(Role = 1)]
        public ActionResult Create()
        {
            ViewBag.Feature = "Thêm mới";
            ViewBag.Element = KeyElement;
            ViewBag.isEdit = false;
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                return View();
            }
        }

        [Permission(Role = 1)]
        public ActionResult Update(Guid id)
        {
            ViewBag.isEdit = true;
            ViewBag.Feature = "Cập nhật";
            ViewBag.Element = KeyElement;
            if (Request.Url != null)
            {
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1));
            }

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var patient = workScope.Articles
                    .FirstOrDefault(x => x.Id == id && !x.IsDelete);

                if (patient != null)
                {
                    return View("Create", patient);
                }
                else
                {
                    return RedirectToAction("Create", "Article");
                }
            }
        }

        [HttpGet]
        public JsonResult GetJson(string query)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var regimens = workScope.Articles.Query(x => x.Title.Contains(query)).Select(x => new
                {
                    value = x.Title,
                    data = x.Id
                }).ToList();

                return Json(new
                {
                    suggestions = regimens
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Permission(Role = 1)]
        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(Article input, bool isEdit)
        {
            try
            {
                var user = CookiesManage.GetUser();
                if (isEdit) //update
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        var elm = workScope.Articles.FirstOrDefault(x => x.Id == input.Id);
                        if (elm != null) //update
                        {
                            input.CreatedDate = elm.CreatedDate;
                            input.ModifiedDate = DateTime.Now;

                            input.CreatedBy = elm.CreatedBy;
                            input.ModifiedBy = user.FullName;

                            elm = input;
                            elm.Content = elm.Content.Replace("§", "o");

                            workScope.Articles.Put(elm, elm.Id);
                            workScope.Complete();
                            return Json(new
                            {
                                status = true,
                                mess = "Cập nhập thành công ",
                            });
                        }

                        return Json(new { status = false, mess = "Không tồn tại " });
                    }
                }

                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    input.Id = Guid.NewGuid();
                    input.CreatedDate = DateTime.Now;
                    input.ModifiedDate = DateTime.Now;

                    input.CreatedBy = user.FullName;
                    input.ModifiedBy = user.FullName;
                    workScope.Articles.Add(input);
                    workScope.Complete();
                }

                return Json(new
                {
                    status = true,
                    mess = "Thêm thành công ",
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    mess = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }

        [Permission(Role = 1)]
        [HttpPost]
        [ValidateInput(true)]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            try
            {
                if (upload?.FileName != null)
                {
                    if (upload.ContentLength >= FileKey.MaxLength)
                    {
                        return Json(new { status = false, mess = "File Max Length" });
                    }
                    var splitFilename = upload.FileName.Split('.');
                    if (splitFilename.Length > 1)
                    {
                        var fileExt = splitFilename[splitFilename.Length - 1];

                        //Check ext

                        if (FileKey.FileExtensionApprove().Any(x => x == fileExt))
                        {
                            var now = DateTime.Now;
                            var yearName = now.ToString("yyyy");
                            var monthName = now.ToString("MMMM");
                            var dayName = now.ToString("dd-MM-yyyy");

                            var folder = Path.Combine(Server.MapPath("~/FileUploads/images/"),
                                Path.Combine(yearName,
                                    Path.Combine(monthName,
                                        dayName)));
                            var createFolder = Directory.CreateDirectory(folder);

                            var slugName = StringHelper.ConvertToAlias(upload.FileName);
                            var fileName = slugName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + fileExt;
                            var path = Path.Combine(createFolder.FullName, fileName);
                            upload.SaveAs(path);

                            return Json(new { status = true, mess = "Cập nhật thành công", url = $"/FileUploads/images/{yearName}/{monthName}/{dayName}/{fileName}" });
                        }
                        return Json(new { status = false, mess = "FileExtensionReject" });
                    }

                    return Json(new { status = false, mess = "FileExtensionReject" });
                }
                return Json(new { status = false, mess = "Cập nhật không thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, mess = "Cập nhật không thành công", ex });
            }
        }

        [Permission(Role = 1)]
        [HttpPost]
        public JsonResult Del(Guid id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.Articles.Get(id);
                    if (elm != null) //update
                    {
                        elm.IsDelete = true;
                        workScope.Articles.Put(elm, elm.Id);
                        workScope.Complete();
                        return Json(new { status = true, mess = "Xóa thành công " });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " });
                    }
                }
            }
            catch
            {
                return Json(new { status = false, mess = "Thất bại" });
            }
        }
    }
}