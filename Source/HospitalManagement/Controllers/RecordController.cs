using BELibrary.Core.Entity;
using BELibrary.DbContext;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Controllers
{
    public class RecordController : BaseController
    {
        // GET: Record
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Account");
        }

        public ActionResult Attachment(Guid detailRecordId)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                //Check isRecord of current user
                var listData = workScope.AttachmentAssigns
                    .Include(x => x.Attachment).Where(x => x.DetailRecordId == detailRecordId).ToList();
                return View(listData);
            }
        }

        public ActionResult Prescription(Guid detailRecordId)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                //Check isRecord of current user
                var listData = workScope.Prescriptions.Include(x => x.DetailPrescription.Medicine)
                    .Where(x => x.DetailRecordId == detailRecordId).ToList();
                return View(listData);
            }
        }
    }
}