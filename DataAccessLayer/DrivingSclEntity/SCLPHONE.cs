//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DrivingSclData
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class SCLPHONE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NB { get; set; }
        public long SCL_NB { get; set; }
        public string PHONE_NO { get; set; }
        public byte PHONE_TYP { get; set; }
    
        public virtual SCHOOL SCHOOL { get; set; }
    }
}