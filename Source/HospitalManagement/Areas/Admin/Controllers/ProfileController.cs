using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using BELibrary.Utils;
using HospitalManagement.Areas.Admin.Authorization;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    [Permission(Role = RoleKey.Doctor)]
    public class ProfileController : BaseController
    {
        // GET: Admin/Profile

        private string _keyElement = "Tài khoản";

        public ActionResult Index()
        {
            ViewBag.Feature = "Hồ sơ";
            ViewBag.Element = _keyElement;
            var user = GetCurrentUser();

            if (user == null)
                return Redirect("/admin");

            if (user.Role != RoleKey.Doctor)
                return View(user);

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                ViewBag.Doctor = workScope.Doctors.Include(x => x.Faculty)
                    .FirstOrDefault(x => x.Id == user.DoctorId);
            }
            return View(user);
        }

        public ActionResult Edit()
        {
            ViewBag.Feature = "Cập nhật";
            ViewBag.Element = _keyElement;
            var user = GetCurrentUser();

            if (user == null)
                return Redirect("/admin");

            ViewBag.Genders = new SelectList(GenderKey.GetDic(), "Value", "Text");

            if (user.Role != RoleKey.Doctor)
                return View(user);

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var doctor = workScope.Doctors.Include(x => x.Faculty)
                    .FirstOrDefault(x => x.Id == user.DoctorId) ?? new Doctor();
                ViewBag.Doctor = doctor;

                var faculties = workScope.Faculties.GetAll().ToList();
                ViewBag.Faculties = new SelectList(faculties, "Id", "Name", selectedValue: doctor.FacultyId);
            }
            return View(user);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateInfo(Account input, Guid? facultyId, string rePassword)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var account = GetCurrentUser();

                    if (account != null) //update
                    {
                        //xu ly password
                        if (!string.IsNullOrEmpty(input.Password) || rePassword != "")
                        {
                            if (!CookiesManage.Logined())
                            {
                                return Json(new { status = false, mess = "Chưa đăng nhập" });
                            }
                            if (input.Password != rePassword)
                            {
                                return Json(new { status = false, mess = "Mật khẩu không khớp" });
                            }

                            var passwordFactory = input.Password + VariableExtensions.KeyCrypto;
                            var passwordCryptor = CryptorEngine.Encrypt(passwordFactory, true);
                            input.Password = passwordCryptor;
                        }
                        else
                        {
                            input.Password = account.Password;
                        }

                        input.Id = account.Id;
                        input.UserName = account.UserName;
                        input.Role = account.Role;
                        input.PatientId = account.PatientId;
                        input.DoctorId = account.DoctorId;

                        account = input;
                        workScope.Accounts.Put(account, account.Id);

                        if (account.Role == RoleKey.Doctor && facultyId.HasValue)
                        {
                            var doctor = workScope.Doctors.FirstOrDefault(x => x.Id == account.DoctorId);
                            if (doctor != null && workScope.Faculties.GetAll().Any(x => x.Id == facultyId))
                            {
                                doctor.FacultyId = facultyId.GetValueOrDefault();
                                workScope.Doctors.Put(doctor, doctor.Id);
                            }
                        }

                        workScope.Complete();

                        return Json(new { status = true, mess = "Cập nhập thành công " });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " + _keyElement });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}