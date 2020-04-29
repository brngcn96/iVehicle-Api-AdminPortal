//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DirectoryService.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Vehicle
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Vehicle()
        {
            this.Report = new HashSet<Report>();
        }
    
        public int Vehicle_ID { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public bool Status { get; set; }
        public string Name { get; set; }
        public bool isReserved { get; set; }
        public int Station_ID { get; set; }
        public int Position_in_Station { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Report> Report { get; set; }
        public virtual Station Station { get; set; }
    }
}
