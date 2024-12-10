using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BELibrary.Entity
{
    [Table("UserVerification")]
    public class UserVerification
    {
        public int Id { get; set; }
        public string Mode { get; set; }
        public string Token { get; set; }
        public DateTime? TokenExpirationDate { get; set; }
        public string VerificationCode { get; set; }
        public DateTime? CodeExpirationDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? Status { get; set; }
        public int Link { get; set; }
        public int? AccountId { get; set; }
        public string Email { get; set; }
        public DateTime DeletedDate { get; set; }
    }
}