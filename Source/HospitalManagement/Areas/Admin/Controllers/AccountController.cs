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
    [Permission(Role = RoleKey.Admin)]
    public class AccountController : BaseController
    {
        // GET: Admin/Course
        private string _keyElement = "Tài khoản";

        public ActionResult Index(int? role)
        {
            role = role ?? RoleKey.Admin;

            ViewBag.Feature = "Danh sách";
            ViewBag.Element = _keyElement;

            if (Request.Url != null) ViewBag.BaseURL = Request.Url.LocalPath;

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var lstRole = RoleKey.GetDic();
                ViewBag.Roles = new SelectList(lstRole, "Value", "Text");
                ViewBag.Role = role;

                switch (role)
                {
                    case RoleKey.Admin:
                        {
                            _keyElement += " - Quản trị";

                            var listData = workScope.Accounts.Query(x => x.Role == RoleKey.Admin && !x.IsDeleted).ToList();
                            return View(listData);
                        }
                    case RoleKey.Doctor:
                        {
                            _keyElement += " - Bác Sĩ";
                            var accountDoctor = workScope.Accounts.Query(x => x.Role == RoleKey.Doctor && !x.IsDeleted).ToList();
                            return View(accountDoctor);
                        }
                    case RoleKey.Patient:
                        {
                            var accountPatient = workScope.Accounts.Query(x => x.Role == RoleKey.Patient && !x.IsDeleted).ToList();
                            return View(accountPatient);
                        }
                    default:
                        {
                            var listData = workScope.Accounts.Query(x => !x.IsDeleted).ToList();
                            return View(listData);
                        }
                }
            }
        }

        public ActionResult Create(int role)
        {
            ViewBag.Feature = "Thêm mới";
            ViewBag.Element = RoleKey.GetRole(role);

            if (Request.Url != null)
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1)) + "?role=" + role;

            ViewBag.isEdit = false;
            ViewBag.Role = role;

            ViewBag.Genders = new SelectList(GenderKey.GetDic(), "Value", "Text");
            ViewBag.Roles = new SelectList(RoleKey.GetDic(), "Value", "Text");

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var listDoctor = workScope.Doctors.GetAll().ToList();
                var accountDoctor = workScope.Accounts.Query(x => x.Role == RoleKey.Doctor && !x.IsDeleted).ToList();

                foreach (var doctor in listDoctor.Where(doctor => accountDoctor.Any(x => x.DoctorId == doctor.Id)))
                {
                    listDoctor = listDoctor.Where(d => d.Id != doctor.Id).ToList();
                }

                var doctors = listDoctor.Select(x => new
                {
                    id = x.Id,
                    FullName = x.Name
                });

                ViewBag.Doctors = new SelectList(doctors, "Id", "FullName");

                //

                var listPatient = workScope.Patients.GetAll().ToList();
                var accountPatient = workScope.Accounts.Query(x => x.Role == RoleKey.Patient && !x.IsDeleted).ToList();

                foreach (var patient in listPatient.Where(patient => accountPatient.Any(x => x.PatientId == patient.Id)))
                {
                    listPatient = listPatient.Where(d => d.Id != patient.Id).ToList();
                }

                var patients = listPatient.Select(x => new
                {
                    id = x.Id,
                    FullName = x.PatientCode + " - " + x.FullName
                });

                ViewBag.Patients = new SelectList(patients, "Id", "FullName");

                return View();
            }
        }

        public ActionResult Update(Guid id, int role)
        {
            ViewBag.isEdit = true;
            ViewBag.Role = role;
            ViewBag.Feature = "Cập nhật";

            if (Request.Url != null)
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1)) + "?role=" + role;

            ViewBag.Element = RoleKey.GetRole(role);
            ViewBag.Genders = new SelectList(GenderKey.GetDic(), "Value", "Text");
            ViewBag.Roles = new SelectList(RoleKey.GetDic(), "Value", "Text");

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                if (role == RoleKey.Doctor)
                {
                    var acc = workScope.Accounts.Include(x => x.Doctor).FirstOrDefault(x => x.Id == id);
                    if (acc != null)
                    {
                        acc.Password = "";
                        return View("Create", acc);
                    }
                }
                else if (role == RoleKey.Patient)
                {
                    var acc = workScope.Accounts.Include(x => x.Patient).FirstOrDefault(x => x.Id == id);
                    if (acc != null)
                    {
                        acc.Password = "";
                        return View("Create", acc);
                    }
                }
                else
                {
                    var acc = workScope.Accounts.FirstOrDefault(x => x.Id == id);
                    acc.Password = "";
                    return View("Create", acc);
                }
            }
            return View("Create");
        }

        [HttpPost]
        public JsonResult GetJson(Guid? id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var account = workScope.Accounts.FirstOrDefault(x => x.Id == id && !x.IsDeleted);

                return account == default ?
                    Json(new
                    {
                        status = false,
                        mess = "Có lỗi xảy ra: "
                    }) :
                    Json(new
                    {
                        status = true,
                        mess = "Lấy thành công " + _keyElement,
                        data = new
                        {
                            account.Id,
                            account.FullName,
                            account.PatientId,
                            account.Phone,
                            account.UserName,
                            account.Gender,
                            account.Role,
                            account.DoctorId
                        }
                    });
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(Account input, bool isEdit, string rePassword)
        {
            try
            {
                if (isEdit) //update
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        var elm = workScope.Accounts.FirstOrDefault(x => !x.IsDeleted && x.Id == input.Id);

                        if (elm != null) //update
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

                                var passwordFactory = input.Password + (input.Role == RoleKey.Patient ? VariableExtensions.KeyCryptorClient : VariableExtensions.KeyCrypto);
                                var passwordCryptor = CryptorEngine.Encrypt(passwordFactory, true);
                                input.Password = passwordCryptor;
                            }
                            else
                            {
                                input.Password = elm.Password;
                            }

                            input.UserName = elm.UserName;
                            input.Role = elm.Role;
                            input.PatientId = elm.PatientId;
                            input.DoctorId = elm.DoctorId;

                            if (input.Role != RoleKey.Admin)
                            {
                                input.Phone = elm.Phone;
                                input.Gender = elm.Gender;
                            }

                            elm = input;

                            workScope.Accounts.Put(elm, elm.Id);
                            workScope.Complete();

                            return Json(new { status = true, mess = "Cập nhập thành công " });
                        }
                        else
                        {
                            return Json(new { status = false, mess = "Không tồn tại " + _keyElement });
                        }
                    }
                }
                else //Thêm mới
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        if (string.IsNullOrEmpty(input.Password) || string.IsNullOrEmpty(rePassword))
                        {
                            return Json(new { status = false, mess = "Không được để trống mật khẩu" });
                        }

                        if (input.Password != rePassword)
                        {
                            return Json(new { status = false, mess = "Mật khẩu không khớp" });
                        }

                        var elm = workScope.Accounts.Query(x => x.UserName.ToLower() == input.UserName.ToLower() && !x.IsDeleted).Any();
                        if (elm)
                        {
                            return Json(new { status = false, mess = "Tên đăng nhập đã tồn tại" });
                        }

                        var passwordFactory = input.Password + (input.Role == RoleKey.Patient ? VariableExtensions.KeyCryptorClient : VariableExtensions.KeyCrypto);
                        var passwordCrypto = CryptorEngine.Encrypt(passwordFactory, true);

                        input.Password = passwordCrypto;
                        input.Id = Guid.NewGuid();

                        if (input.Role == RoleKey.Patient)
                        {
                            var patient = workScope.Patients.FirstOrDefault(x => x.Id == input.PatientId);

                            if (patient == null)
                            {
                                return Json(new { status = false, mess = "Bệnh nhân k tồn tại" });
                            }

                            input.PatientId = patient.Id;
                            input.DoctorId = null;
                            input.Phone = patient.Phone;
                            input.Gender = patient.Gender;

                            workScope.Accounts.Add(input);
                            workScope.Complete();
                        }
                        else if (input.Role == RoleKey.Doctor)
                        {
                            var doctor = workScope.Doctors.FirstOrDefault(x => x.Id == input.DoctorId);

                            if (doctor == null)
                            {
                                return Json(new { status = false, mess = "Bác sĩ k tồn tại" });
                            }

                            input.PatientId = null;
                            input.DoctorId = doctor.Id;
                            input.Phone = doctor.Phone;
                            input.Gender = doctor.Gender;

                            workScope.Accounts.Add(input);
                            workScope.Complete();
                        }
                        else if (input.Role == RoleKey.Admin)
                        {
                            input.PatientId = null;
                            input.DoctorId = null;

                            workScope.Accounts.Add(input);
                            workScope.Complete();
                        }
                    }
                    return Json(new { status = true, mess = "Thêm thành công " + _keyElement });
                }
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

        [HttpPost]
        public JsonResult Del(Guid id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.Accounts.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                    if (elm != null)
                    {
                        elm.IsDeleted = true;
                        //del
                        workScope.Accounts.Put(elm, elm.Id);
                        workScope.Complete();
                        return Json(new { status = true, mess = "Xóa thành công " + _keyElement });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " + _keyElement });
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