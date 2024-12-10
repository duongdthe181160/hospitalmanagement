using System;

namespace BELibrary.Entity
{
    public class AttachmentAssign
    {
        public Guid Id { get; set; }

        public Guid AttachmentId { get; set; }

        public Guid DetailRecordId { get; set; }

        public virtual Attachment Attachment { get; set; }

        public virtual DetailRecord DetailRecord { get; set; }
    }
}