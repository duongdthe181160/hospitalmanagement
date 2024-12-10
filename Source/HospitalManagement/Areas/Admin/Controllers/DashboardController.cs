using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class DashboardController : BaseController
    {
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            ViewBag.Element = "Hệ thống";
            ViewBag.Feature = "Bảng điều khiển";
            if (Request.Url != null) ViewBag.BaseURL = Request.Url.LocalPath;

            var user = GetCurrentUser();

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var documents = workScope.Attachments.GetAll().ToList();

                var patients = workScope.Patients.GetAll().ToList();

                if (user.Role == RoleKey.Doctor)
                {
                    var patientOfDoctors =
                        workScope.PatientDoctors.Query(x => x.DoctorId == user.DoctorId) ?? new List<PatientDoctor>();

                    var patientOfDoctorIds = patientOfDoctors.Select(x => x.PatientId);

                    patients = patients.Where(x => patientOfDoctorIds.Contains(x.Id)).ToList();
                }

                var prescriptions = workScope.Prescriptions.GetAll().ToList();

                var items = workScope.Items.GetAll().ToList();

                var schedules = workScope.DoctorSchedules.GetAll().ToList();

                if (user.Role == RoleKey.Doctor)
                {
                    schedules = schedules.Where(x => x.DoctorId == user.DoctorId).ToList();
                }
                //
                ViewBag.DocumentCount = documents.Count;
                ViewBag.PatientCount = patients.Count;
                ViewBag.PrescriptionCount = prescriptions.Count;
                ViewBag.ItemCount = items.Count;
                ViewBag.ScheduleCount = schedules.Count;

                var now = DateTime.Now;
                //
                ViewBag.DocumentTodayCount = documents.Count(x => x.ModifiedDate.Day == now.Day && x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);
                ViewBag.DocumentMonthCount = documents.Count(x => x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);

                ViewBag.ScheduleTodayCount = schedules.Count(x => x.ScheduleBook.Day == now.Day && x.ScheduleBook.Month == now.Month && x.ScheduleBook.Year == now.Year);
                ViewBag.ScheduleMonthCount = schedules.Count(x => x.ScheduleBook.Month == now.Month && x.ScheduleBook.Year == now.Year);

                ViewBag.PatientTodayCount = patients.Count(x => x.JoinDate.Day == now.Day && x.JoinDate.Month == now.Month && x.JoinDate.Year == now.Year);
                ViewBag.PatientMonthCount = patients.Count(x => x.JoinDate.Month == now.Month && x.JoinDate.Year == now.Year);

                ViewBag.PrescriptionTodayCount = prescriptions.Count(x => x.ModifiedDate.Day == now.Day && x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);
                ViewBag.PrescriptionMonthCount = documents.Count(x => x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);

                ViewBag.ItemTodayCount = items.Count(x => x.ModifiedDate.Day == now.Day && x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);
                ViewBag.ItemMonthCount = items.Count(x => x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);

                // new patient

                var patientsNew = workScope.Patients.Query(x => x.Status).OrderByDescending(x => x.JoinDate).Take(6).ToList();

                if (user.Role == RoleKey.Doctor)
                {
                    var patientOfDoctors =
                        workScope.PatientDoctors.Query(x => x.DoctorId == user.DoctorId) ?? new List<PatientDoctor>();

                    var patientOfDoctorIds = patientOfDoctors.Select(x => x.PatientId);

                    patientsNew = patientsNew.Where(x => patientOfDoctorIds.Contains(x.Id)).ToList();
                }

                ViewBag.PatientsNew = patientsNew;

                var categories = workScope.Categories.GetAll().ToList();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
            }

            return View();
        }
    }
}