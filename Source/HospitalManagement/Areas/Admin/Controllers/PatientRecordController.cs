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
    public class PatientRecordController : BaseController
    {
        // GET: Admin/Patient
        private readonly string KeyElement = "Danh sách khám bệnh";

        public ActionResult Index(Guid? patientId)
        {
            ViewBag.Feature = "Bệnh án";
            ViewBag.Element = KeyElement;
            ViewBag.BaseURL = "/Admin/Patient";

            var user = GetCurrentUser();

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                List<PatientRecord> listData;
                var patients = workScope.Patients.GetAll();

                if (user.Role == RoleKey.Doctor)
                {
                    var patientOfDoctors =
                        workScope.PatientDoctors.Query(x => x.DoctorId == user.DoctorId) ?? new List<PatientDoctor>();

                    var patientOfDoctorIds = patientOfDoctors.Select(x => x.PatientId);

                    patients = patients.Where(x => patientOfDoctorIds.Contains(x.Id)).ToList();
                }

                if (patientId.HasValue)
                {
                    var patient = workScope.Patients.FirstOrDefault(x => x.Id == patientId);
                    ViewBag.Patient = patient;
                    listData =
                      workScope.PatientRecords.Query(x => x.PatientId == patientId && !x.IsDelete).OrderByDescending(x => x.TestDate)
                          .ToList();

                    ViewBag.Patients = new SelectList(patients.Select(x => new
                    {
                        id = x.Id,
                        FullName = x.PatientCode + " - " + x.FullName
                    }), "Id", "FullName", patientId);

                    if (user.Role == RoleKey.Doctor)
                    {
                        listData = listData.Where(x => x.DoctorId == user.DoctorId).ToList();
                    }
                    return View(listData);
                }
                ViewBag.Patients = new SelectList(patients.Select(x => new
                {
                    id = x.Id,
                    FullName = x.PatientCode + " - " + x.FullName
                }), "Id", "FullName");

                listData = workScope.PatientRecords.Query(x => !x.IsDelete).OrderByDescending(x => x.TestDate).ToList();

                if (user.Role == RoleKey.Doctor)
                {
                    listData = listData.Where(x => x.DoctorId == user.DoctorId).ToList();
                }

                return View(listData);
            }
        }

        public ActionResult Create(Guid? patientId)
        {
            var user = GetCurrentUser();
            ViewBag.Feature = "Thêm mới";
            ViewBag.Element = KeyElement;

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                if (Request.Url != null)
                    ViewBag.BaseURL = patientId.HasValue ? "/admin/patientRecord?patientId=" + patientId : "/admin/patientRecord";

                var patients = workScope.Patients.Query(x => !x.IsDeleted).ToList();

                if (user.Role == RoleKey.Doctor)
                {
                    var patientOfDoctors =
                        workScope.PatientDoctors.Query(x => x.DoctorId == user.DoctorId) ?? new List<PatientDoctor>();

                    var patientOfDoctorIds = patientOfDoctors.Select(x => x.PatientId);

                    patients = patients.Where(x => patientOfDoctorIds.Contains(x.Id)).ToList();
                }

                var patientsSelect = patients.Select(x => new
                {
                    id = x.Id,
                    FullName = x.PatientCode + " - " + x.FullName
                });
                ViewBag.Patients = new SelectList(patientsSelect, "Id", "FullName", patientId);

                var doctors = workScope.Doctors.GetAll().Select(x => new
                {
                    id = x.Id,
                    FullName = x.Name
                });

                ViewBag.Doctors = user.Role == RoleKey.Doctor ?
                      new SelectList(doctors, "Id", "FullName", user.DoctorId)
                    : new SelectList(doctors, "Id", "FullName");

                ViewBag.isEdit = false;

                var patientRecord = new PatientRecord
                {
                    BloodVessel = 0,
                    BodyTemperature = 0,
                    Breathing = 0,
                    ClinicalSymptoms = "Bình Thường",
                    DiagnosingLeftEyes = "Bình Thường",
                    DiagnosingRightEyes = "Bình Thường",
                    DiagnosingTwoEyes = "Bình Thường",
                    EyePressureLeft = "0",
                    EyePressureRight = "0",
                    Height = 0,
                    Weight = 0,
                    VisionWithGlassHoleLeft = "10/10",
                    VisionWithGlassHoleRight = "10/10",
                    VisionWithGlassLeft = "10/10",
                    VisionWithGlassRight = "10/10",
                    VisionWithoutGlassesLeft = "10/10",
                    VisionWithoutGlassesRight = "10/10",
                    TestDate = DateTime.UtcNow.AddHours(7)
                };

                return View(patientRecord);
            }
        }

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
                var doctors = workScope.Doctors.GetAll().Select(x => new
                {
                    id = x.Id,
                    FullName = x.Name
                });

                ViewBag.Doctors = new SelectList(doctors, "Id", "FullName");

                var patientRecord = workScope.PatientRecords
                    .FirstOrDefault(x => x.Id == id);

                if (patientRecord == null)
                    return RedirectToAction("Create", "PatientRecord");

                var patients = workScope.Patients.GetAll().Select(x => new
                {
                    id = x.Id,
                    FullName = x.PatientCode + " - " + x.FullName
                });
                ViewBag.Patients = new SelectList(patients, "Id", "FullName", patientRecord.PatientId);

                return View("Create", patientRecord);
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(PatientRecord input, TimeSpan testTime, bool isEdit)
        {
            try
            {
                var user = GetCurrentUser();

                if (isEdit) //update
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        var elm = workScope.PatientRecords.FirstOrDefault(x => x.Id == input.Id && !x.IsDelete);
                        if (elm != null) //update
                        {
                            input.RecordId = elm.RecordId;
                            input.TestDate = input.TestDate.Date + testTime;
                            input.PatientId = elm.PatientId;
                            input.DoctorId = user.Role == RoleKey.Doctor ? user.DoctorId : input.DoctorId;
                            elm = input;

                            workScope.PatientRecords.Put(elm, elm.Id);
                            workScope.Complete();
                            return Json(new
                            {
                                status = true,
                                mess = "Cập nhập thành công ",
                                data = new
                                {
                                    input.RecordId
                                }
                            });
                        }

                        return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                    }
                }

                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var recordId = Guid.NewGuid();

                    var record = new Record
                    {
                        Id = recordId,
                        CreatedDate = DateTime.UtcNow.AddHours(7),
                        ModifiedDate = DateTime.UtcNow.AddHours(7),
                        CreatedBy = GetCurrentUser().FullName,
                        ModifiedBy = GetCurrentUser().FullName,
                        DoctorId = user.Role == RoleKey.Doctor ? user.DoctorId : null
                    };
                    workScope.Records.Add(record);
                    workScope.Complete();

                    var mainRecordDetail = new DetailRecord
                    {
                        Id = Guid.NewGuid(),
                        RecordId = recordId,
                        DiseaseName = input.Title,
                        IsMainRecord = true,
                        Status = true,
                        Process = 0,
                        DoctorId = user.Role == RoleKey.Doctor ? user.DoctorId : null
                    };
                    workScope.DetailRecords.Add(mainRecordDetail);
                    workScope.Complete();

                    input.Id = Guid.NewGuid();
                    input.RecordId = recordId;
                    input.DoctorId = user.Role == RoleKey.Doctor ? user.DoctorId : input.DoctorId;
                    input.TestDate = input.TestDate.Date + testTime;
                    workScope.PatientRecords.Add(input);
                    workScope.Complete();
                }

                return Json(new
                {
                    status = true,
                    mess = "Thêm thành công " + KeyElement,
                    data = new
                    {
                        input.RecordId
                    }
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

        [HttpPost]
        public JsonResult Del(Guid id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.PatientRecords.FirstOrDefault(x => x.Id == id && !x.IsDelete);
                    if (elm != null)
                    {
                        //del Patient
                        var record = workScope.Records.FirstOrDefault(x => x.Id == elm.RecordId);

                        var detailRecords = record.DetailRecords.Where(x => x.RecordId == record.Id);

                        foreach (var detailRecord in detailRecords)
                        {
                            if (workScope.Prescriptions.Query(x => x.DetailRecordId == detailRecord.Id).Any())
                            {
                                return Json(new { status = false, mess = "Không thể xóa, vì còn thuốc" + KeyElement });
                            }
                        }

                        elm.IsDelete = true;
                        workScope.PatientRecords.Put(elm, elm.Id);

                        workScope.Complete();
                        return Json(new { status = true, mess = "Xóa thành công " + KeyElement });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
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