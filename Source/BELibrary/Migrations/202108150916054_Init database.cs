namespace BELibrary.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Initdatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Account",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    FullName = c.String(maxLength: 50),
                    Phone = c.String(maxLength: 15),
                    UserName = c.String(maxLength: 50),
                    LinkAvatar = c.String(maxLength: 250),
                    Gender = c.Boolean(nullable: false),
                    Password = c.String(maxLength: 250),
                    Role = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    PatientId = c.Guid(),
                    DoctorId = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctor", t => t.DoctorId)
                .ForeignKey("dbo.Patient", t => t.PatientId)
                .Index(t => t.PatientId)
                .Index(t => t.DoctorId);

            CreateTable(
                "dbo.Doctor",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(),
                    Avatar = c.String(),
                    Descriptions = c.String(),
                    Address = c.String(),
                    Gender = c.Boolean(nullable: false),
                    Phone = c.String(),
                    Email = c.String(),
                    IsDelete = c.Boolean(nullable: false),
                    FacultyId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Faculty", t => t.FacultyId, cascadeDelete: true)
                .Index(t => t.FacultyId);

            CreateTable(
                "dbo.Faculty",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Patient",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    FullName = c.String(nullable: false, maxLength: 100),
                    DateOfBirth = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Address = c.String(nullable: false, maxLength: 250),
                    Gender = c.Boolean(nullable: false),
                    IndentificationCardId = c.String(maxLength: 20, unicode: false),
                    Phone = c.String(nullable: false, maxLength: 15, unicode: false),
                    Status = c.Boolean(nullable: false),
                    ImageProfile = c.String(),
                    PatientCode = c.String(nullable: false),
                    JoinDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    IndentificationCardDate = c.DateTime(nullable: false),
                    Job = c.String(),
                    WorkPlace = c.String(),
                    HistoryOfIllnessFamily = c.String(),
                    HistoryOfIllnessYourself = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    Email = c.String(maxLength: 250),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.MedicalSupplies",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    DateOfHire = c.DateTime(nullable: false),
                    Status = c.Int(nullable: false),
                    Amount = c.Int(nullable: false),
                    ItemId = c.Guid(nullable: false),
                    PatientId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Item", t => t.ItemId, cascadeDelete: true)
                .ForeignKey("dbo.Patient", t => t.PatientId)
                .Index(t => t.ItemId)
                .Index(t => t.PatientId);

            CreateTable(
                "dbo.Item",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(nullable: false, maxLength: 250),
                    Amount = c.Int(nullable: false),
                    Description = c.String(),
                    CategoryId = c.Guid(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedDate = c.DateTime(nullable: false),
                    ModifiedBy = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.CategoryId)
                .Index(t => t.CategoryId);

            CreateTable(
                "dbo.Category",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(nullable: false, maxLength: 200),
                    Unit = c.String(nullable: false, maxLength: 50),
                    Description = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.AttachmentAssigns",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    AttachmentId = c.Guid(nullable: false),
                    DetailRecordId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attachments", t => t.AttachmentId, cascadeDelete: true)
                .ForeignKey("dbo.DetailRecord", t => t.DetailRecordId, cascadeDelete: true)
                .Index(t => t.AttachmentId)
                .Index(t => t.DetailRecordId);

            CreateTable(
                "dbo.Attachments",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(),
                    Type = c.String(),
                    Url = c.String(),
                    CreatedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedDate = c.DateTime(nullable: false),
                    ModifiedBy = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.DetailRecord",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    DiseaseName = c.String(nullable: false, maxLength: 200),
                    Note = c.String(),
                    Result = c.String(),
                    Status = c.Boolean(nullable: false),
                    DoctorId = c.Guid(),
                    FacultyId = c.Guid(),
                    Process = c.Int(nullable: false),
                    RecordId = c.Guid(nullable: false),
                    IsMainRecord = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Faculty", t => t.FacultyId)
                .ForeignKey("dbo.Record", t => t.RecordId)
                .Index(t => t.FacultyId)
                .Index(t => t.RecordId);

            CreateTable(
                "dbo.Record",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedDate = c.DateTime(nullable: false),
                    ModifiedBy = c.String(),
                    DoctorId = c.Guid(),
                    Note = c.String(),
                    Result = c.String(),
                    StatusRecord = c.Int(nullable: false),
                    IsDelete = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctor", t => t.DoctorId)
                .Index(t => t.DoctorId);

            CreateTable(
                "dbo.DetailPrescription",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Amount = c.Int(nullable: false),
                    Unit = c.String(nullable: false, maxLength: 50),
                    Note = c.String(nullable: false),
                    MedicineId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Medicine", t => t.MedicineId)
                .Index(t => t.MedicineId);

            CreateTable(
                "dbo.Medicine",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(nullable: false, maxLength: 200),
                    Description = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.DoctorSchedule",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    DoctorId = c.Guid(nullable: false),
                    PatientId = c.Guid(nullable: false),
                    ScheduleBook = c.DateTime(nullable: false),
                    Status = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .ForeignKey("dbo.Patient", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.DoctorId)
                .Index(t => t.PatientId);

            CreateTable(
                "dbo.PatientDoctor",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    DoctorId = c.Guid(nullable: false),
                    PatientId = c.Guid(nullable: false),
                    Status = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .ForeignKey("dbo.Patient", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.DoctorId)
                .Index(t => t.PatientId);

            CreateTable(
                "dbo.PatientRecord",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Title = c.String(),
                    TestDate = c.DateTime(nullable: false),
                    BloodVessel = c.Double(nullable: false),
                    BodyTemperature = c.Double(nullable: false),
                    Height = c.Double(nullable: false),
                    Weight = c.Double(nullable: false),
                    Breathing = c.Double(nullable: false),
                    VisionWithoutGlassesRight = c.String(),
                    VisionWithoutGlassesLeft = c.String(),
                    VisionWithGlassHoleLeft = c.String(),
                    VisionWithGlassHoleRight = c.String(),
                    VisionWithGlassLeft = c.String(),
                    VisionWithGlassRight = c.String(),
                    EyePressureRight = c.String(),
                    EyePressureLeft = c.String(),
                    ClinicalSymptoms = c.String(),
                    DiagnosingTwoEyes = c.String(),
                    DiagnosingLeftEyes = c.String(),
                    DiagnosingRightEyes = c.String(),
                    Status = c.Boolean(nullable: false),
                    IsDelete = c.Boolean(nullable: false),
                    DoctorId = c.Guid(),
                    RecordId = c.Guid(nullable: false),
                    PatientId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctor", t => t.DoctorId)
                .ForeignKey("dbo.Patient", t => t.PatientId, cascadeDelete: true)
                .ForeignKey("dbo.Record", t => t.RecordId, cascadeDelete: true)
                .Index(t => t.DoctorId)
                .Index(t => t.RecordId)
                .Index(t => t.PatientId);

            CreateTable(
                "dbo.Prescription",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    DetailPrescriptionId = c.Guid(nullable: false),
                    DetailRecordId = c.Guid(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedDate = c.DateTime(nullable: false),
                    ModifiedBy = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DetailPrescription", t => t.DetailPrescriptionId, cascadeDelete: true)
                .ForeignKey("dbo.DetailRecord", t => t.DetailRecordId, cascadeDelete: true)
                .Index(t => t.DetailPrescriptionId)
                .Index(t => t.DetailRecordId);

            CreateTable(
                "dbo.UserVerification",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Mode = c.String(),
                    Token = c.String(),
                    TokenExpirationDate = c.DateTime(),
                    VerificationCode = c.String(),
                    CodeExpirationDate = c.DateTime(),
                    CreatedDate = c.DateTime(nullable: false),
                    Status = c.Int(),
                    Link = c.Int(nullable: false),
                    AccountId = c.Int(),
                    Email = c.String(),
                    DeletedDate = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Prescription", "DetailRecordId", "dbo.DetailRecord");
            DropForeignKey("dbo.Prescription", "DetailPrescriptionId", "dbo.DetailPrescription");
            DropForeignKey("dbo.PatientRecord", "RecordId", "dbo.Record");
            DropForeignKey("dbo.PatientRecord", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.PatientRecord", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.PatientDoctor", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.PatientDoctor", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DoctorSchedule", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.DoctorSchedule", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DetailPrescription", "MedicineId", "dbo.Medicine");
            DropForeignKey("dbo.AttachmentAssigns", "DetailRecordId", "dbo.DetailRecord");
            DropForeignKey("dbo.Record", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DetailRecord", "RecordId", "dbo.Record");
            DropForeignKey("dbo.DetailRecord", "FacultyId", "dbo.Faculty");
            DropForeignKey("dbo.AttachmentAssigns", "AttachmentId", "dbo.Attachments");
            DropForeignKey("dbo.MedicalSupplies", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.MedicalSupplies", "ItemId", "dbo.Item");
            DropForeignKey("dbo.Item", "CategoryId", "dbo.Category");
            DropForeignKey("dbo.Account", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.Account", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.Doctor", "FacultyId", "dbo.Faculty");
            DropIndex("dbo.Prescription", new[] { "DetailRecordId" });
            DropIndex("dbo.Prescription", new[] { "DetailPrescriptionId" });
            DropIndex("dbo.PatientRecord", new[] { "PatientId" });
            DropIndex("dbo.PatientRecord", new[] { "RecordId" });
            DropIndex("dbo.PatientRecord", new[] { "DoctorId" });
            DropIndex("dbo.PatientDoctor", new[] { "PatientId" });
            DropIndex("dbo.PatientDoctor", new[] { "DoctorId" });
            DropIndex("dbo.DoctorSchedule", new[] { "PatientId" });
            DropIndex("dbo.DoctorSchedule", new[] { "DoctorId" });
            DropIndex("dbo.DetailPrescription", new[] { "MedicineId" });
            DropIndex("dbo.Record", new[] { "DoctorId" });
            DropIndex("dbo.DetailRecord", new[] { "RecordId" });
            DropIndex("dbo.DetailRecord", new[] { "FacultyId" });
            DropIndex("dbo.AttachmentAssigns", new[] { "DetailRecordId" });
            DropIndex("dbo.AttachmentAssigns", new[] { "AttachmentId" });
            DropIndex("dbo.Item", new[] { "CategoryId" });
            DropIndex("dbo.MedicalSupplies", new[] { "PatientId" });
            DropIndex("dbo.MedicalSupplies", new[] { "ItemId" });
            DropIndex("dbo.Doctor", new[] { "FacultyId" });
            DropIndex("dbo.Account", new[] { "DoctorId" });
            DropIndex("dbo.Account", new[] { "PatientId" });
            DropTable("dbo.UserVerification");
            DropTable("dbo.Prescription");
            DropTable("dbo.PatientRecord");
            DropTable("dbo.PatientDoctor");
            DropTable("dbo.DoctorSchedule");
            DropTable("dbo.Medicine");
            DropTable("dbo.DetailPrescription");
            DropTable("dbo.Record");
            DropTable("dbo.DetailRecord");
            DropTable("dbo.Attachments");
            DropTable("dbo.AttachmentAssigns");
            DropTable("dbo.Category");
            DropTable("dbo.Item");
            DropTable("dbo.MedicalSupplies");
            DropTable("dbo.Patient");
            DropTable("dbo.Faculty");
            DropTable("dbo.Doctor");
            DropTable("dbo.Account");
        }
    }
}