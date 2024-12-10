using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HospitalManagement.Handler;

namespace HospitalManagement.Controllers
{
    public class DoctorController : Controller
    {
        // GET: Doctor
        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        public ActionResult Search(string query, int? page, List<bool> genders, List<Guid> facultiesSelected)
        {
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
                var listData = workScope.Doctors.Query(x => !x.IsDelete).ToList();
                var faculties = workScope.Faculties.GetAll().ToList();
                ViewBag.Faculties = faculties;

                double elapsedMs = 0;

                var q = (
                    from mt in listData
                    join f in faculties on mt.FacultyId equals f.Id

                    select mt).AsQueryable();

                if (!string.IsNullOrEmpty(query))
                    q = q.Where(x => x.Name.ToLower().Contains(query.Trim().ToLower())
                                     || !string.IsNullOrEmpty(x.Descriptions) && x.Descriptions.ToLower().Contains(query.Trim().ToLower())
                                     || !string.IsNullOrEmpty(x.Faculty.Name) && x.Faculty.Name.ToLower().Contains(query.Trim().ToLower())

                    );

                if (facultiesSelected != null && facultiesSelected.Count > 0)
                    q = q.Where(x => facultiesSelected.Contains(x.Faculty.Id));
                if (genders != null && genders.Count > 0)
                    q = q.Where(x => genders != null && genders.Contains(x.Gender));

                ViewBag.Total = q.Count();
                ViewBag.FacultiesSelected = facultiesSelected;
                ViewBag.Genders = genders;
                watch.Stop();

                elapsedMs = (double)watch.ElapsedMilliseconds / 1000;
                ViewBag.RequestTime = elapsedMs;
                return View(q.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult Detail(Guid id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var doctor = workScope.Doctors.Include(x => x.Faculty).FirstOrDefault(x => x.Id == id);
                if (doctor != null)
                {
                    return View(doctor);
                }
                else
                {
                    return RedirectToAction("Search");
                }
            }
        }

        [HttpPost]
        public JsonResult GetJson(Guid? id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var doctor = workScope.Doctors.FirstOrDefault(x => x.Id == id && !x.IsDelete);

                return doctor == default ?
                    Json(new
                    {
                        status = false,
                        mess = "Có lỗi xảy ra: "
                    }) :
                    Json(new
                    {
                        status = true,
                        mess = "Lấy thành công ",
                        data = new
                        {
                            doctor.Id,
                            doctor.Name,
                            doctor.Address,
                            doctor.Email,
                            doctor.Phone,
                            doctor.FacultyId,
                            doctor.Gender,
                            doctor.Avatar,
                            doctor.Descriptions
                        }
                    });
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult Book(Guid doctorId, DateTime time)
        {
            var user = CookiesManage.GetUser();

            if (user == null)
            {
                return Json(new { status = false, mess = "Vui lòng đăng nhập" });
            }

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var doctor = workScope.Doctors.FirstOrDefault(x => x.Id == doctorId && !x.IsDelete);
                if (doctor == null)
                    return Json(new { status = true, mess = "Cập nhập thành công " });

                workScope.DoctorSchedules.Add(new DoctorSchedule
                {
                    DoctorId = doctor.Id,
                    PatientId = user.PatientId.GetValueOrDefault(),
                    ScheduleBook = time,
                    Status = BookingStatusKey.Pending
                });
                workScope.Complete();

                var patientDoctor = workScope.PatientDoctors.FirstOrDefault(x =>
                    x.DoctorId == doctor.Id && x.PatientId == user.PatientId);

                if (patientDoctor != null)
                    return Json(new { status = true, mess = "Cập nhập thành công " });

                workScope.PatientDoctors.Add(new PatientDoctor
                {
                    PatientId = user.PatientId.GetValueOrDefault(),
                    DoctorId = doctor.Id,
                    Status = 1
                });
                workScope.Complete();

                return Json(new { status = true, mess = "Cập nhập thành công " });
            }
        }
    }
}