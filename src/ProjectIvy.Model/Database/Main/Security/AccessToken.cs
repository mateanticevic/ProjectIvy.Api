﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Security
{
    [Table("AccessToken", Schema = "Security")]
    public class AccessToken : UserEntity
    {
        [Key]
        public int Id { get; set; }

        public bool IsActive { get; set; }

        public string Token { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidUntil { get; set; }

        public string UserAgent { get; set; }

        public string OperatingSystem { get; set; }

        public string IpAddress { get; set; }

        public string Bearer { get; set; }

        public long? IpAddressValue { get; set; }
    }
}
