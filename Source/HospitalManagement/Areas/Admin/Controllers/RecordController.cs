using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class RecordController : BaseController
    {
        private readonly string KeyElement = "Bệnh án";

        // GET: Admin/Record
        public ActionResult Index(Guid id)
        {
            ViewBag.Feature = "Thêm mới";
            ViewBag.Element = KeyElement;

            var user = GetCurrentUser();

            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var patientRecord = workScope.PatientRecords.FirstOrDefault(x => x.RecordId == id && !x.IsDelete);

                    if (user.Role == RoleKey.Doctor && patientRecord.DoctorId != user.DoctorId)
                    {
                        return RedirectToAction("E401", "Login");
                    }

                    if (patientRecord == null)
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }

                    var record = workScope.Records.FirstOrDefault(x => x.Id == patientRecord.RecordId);
                    var patient = workScope.Patients.FirstOrDefault(x => x.Id == patientRecord.PatientId);

                    if (record == null)
                    {
                        record = new Record
                        {
                            Id = Guid.NewGuid(),
                            CreatedBy = GetCurrentUser().FullName,
                            ModifiedBy = GetCurrentUser().FullName,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            Note = "",
                            Result = "",
                            DoctorId = user.Role == RoleKey.Doctor ? user.DoctorId : null
                        };
                        workScope.Records.Add(record);
                        workScope.Complete();
                    }
                    ViewBag.Record = record;

                    var doctors = workScope.Doctors.GetAll().ToList();
                    ViewBag.Doctors = new SelectList(doctors, "Id", "Name");

                    var faculties = workScope.Faculties.GetAll().ToList();
                    ViewBag.Faculties = new SelectList(faculties, "Id", "Name");

                    var lstStatus = StatusRecord.GetDic();
                    ViewBag.ListStatus = new SelectList(lstStatus, "Value", "Text");

                    var detailRecords = workScope.DetailRecords.Query(x => x.RecordId == record.Id && !x.IsMainRecord).OrderByDescending(x => x.Process).ToList();

                    var mainDetailRecords =
                        workScope.DetailRecords.FirstOrDefault(x => x.RecordId == record.Id && x.IsMainRecord);

                    ViewBag.MainDetailRecords = mainDetailRecords;
                    ViewBag.Patient = patient;
                    ViewBag.DetailRecords = detailRecords;

                    ViewBag.BaseURL = "/admin/patientRecord?patientId=" + patient.Id;
                    return View(patientRecord);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost]
        public JsonResult GetJson(Guid id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var detailRecord = workScope.DetailRecords.FirstOrDefault(x => x.Id == id);

                var user = GetCurrentUser();

                if (user.Role == RoleKey.Doctor && detailRecord.DoctorId != user.DoctorId)
                {
                    return Json(new
                    {
                        status = false,
                        mess = "K có quyền"
                    });
                }
                return detailRecord == null ?
                    Json(new
                    {
                        status = false,
                        mess = "Có lỗi xảy ra: "
                    }) :
                    Json(new
                    {
                        status = true,
                        mess = "Lấy thành công " + KeyElement,
                        data = new
                        {
                            detailRecord.Id,
                            detailRecord.DiseaseName,
                            detailRecord.FacultyId,
                            detailRecord.DoctorId,
                            detailRecord.Note,
                            detailRecord.Status,
                            detailRecord.Result,
                            detailRecord.Process
                        }
                    });
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(DetailRecord input, Guid detailDoctorId, bool isEdit)
        {
            try
            {
                var user = GetCurrentUser();
                if (isEdit) //update
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        var elm = workScope.DetailRecords.Get(input.Id);

                        if (elm != null) //update
                        {
                            //Che bien du lieu

                            elm = input;
                            elm.DoctorId = user.Role == RoleKey.Doctor ? user.DoctorId : detailDoctorId;

                            workScope.DetailRecords.Put(elm, elm.Id);
                            workScope.Complete();

                            //attachments handle

                            return Json(new
                            {
                                status = true,
                                mess = "Cập nhập thành công ",
                                data = new
                                {
                                    detailRecordId = input.Id
                                }
                            });
                        }
                        else
                        {
                            return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                        }
                    }
                }
                else //Thêm mới
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        //Che bien du lieu
                        input.Id = Guid.NewGuid();
                        input.DoctorId = user.Role == RoleKey.Doctor ? user.DoctorId : detailDoctorId;

                        workScope.DetailRecords.Add(input);
                        workScope.Complete();
                    }
                    return Json(new
                    {
                        status = true,
                        mess = "Thêm thành công " + KeyElement,
                        data = new
                        {
                            detailRecordId = input.Id
                        }
                    });
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

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateRecord(Record input)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.Records.FirstOrDefault(x => x.Id == input.Id && !x.IsDelete);

                    if (elm != null) //update
                    {
                        //Che bien du lieu
                        input.CreatedBy = elm.CreatedBy;
                        input.CreatedDate = elm.CreatedDate;
                        input.ModifiedBy = GetCurrentUser().FullName;
                        input.ModifiedDate = DateTime.Now;

                        elm = input;

                        workScope.Records.Put(elm, elm.Id);
                        workScope.Complete();

                        return Json(new { status = true, mess = "Cập nhập thành công " });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                    }
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
                    var elm = workScope.Records.FirstOrDefault(x => x.Id == id && !x.IsDelete);

                    var user = GetCurrentUser();

                    if (user.Role == RoleKey.Doctor && elm.DoctorId != user.DoctorId)
                    {
                        return Json(new
                        {
                            status = false,
                            mess = "K có quyền"
                        });
                    }

                    if (elm != null)
                    {
                        elm.IsDelete = true;
                        //del
                        workScope.Records.Put(elm, elm.Id);
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