namespace CMS.Configuration.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Event")]
    public partial class Event
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Event()
        {
            CancelledEvents = new HashSet<CancelledEvent>();
        }

        public int Id { get; set; }

        public string BackgroundColor { get; set; }

        public string BackgroundColorDescriptive { get; set; }

        public string BorderColor { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime End { get; set; }

        public int? ModuleId { get; set; }

        public string Room { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Start { get; set; }

        [Required]
        public string Title { get; set; }

        public int TimetableId { get; set; }

        public bool Repeatable { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CancelledEvent> CancelledEvents { get; set; }

        public virtual Module Module { get; set; }

        public virtual Timetable Timetable { get; set; }
    }
}
