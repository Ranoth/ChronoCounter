﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChronoCounter.DBModels
{
    [Index("Id", IsUnique = true)]
    public partial class Chronos
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public long Time { get; set; }
        public long SessionId { get; set; }
        public long Number { get; set; }

        [ForeignKey("SessionId")]
        [InverseProperty("Chronos")]
        public virtual Session Session { get; set; }
    }
}