namespace BELibrary.Entity
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Article")]
    public partial class Article
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public bool IsDelete { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }
    }
}