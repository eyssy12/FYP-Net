namespace CMS.Configuration.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LecturerModule")]
    public partial class LecturerModule
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int LecturerId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ModuleId { get; set; }

        public int Id { get; set; }

        public virtual Lecturer Lecturer { get; set; }

        public virtual Module Module { get; set; }
    }
}
