//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DrivingSclData
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("DRIVINGSCHOOLS.ZPRSTYPE")]
    public partial class ZPRSTYPE
    {
        public ZPRSTYPE()
        {
            this.ZPERSON = new HashSet<ZPERSON>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NB { get; set; }
        public string TYPNAME { get; set; }
    
        public virtual ICollection<ZPERSON> ZPERSON { get; set; }
    }
}