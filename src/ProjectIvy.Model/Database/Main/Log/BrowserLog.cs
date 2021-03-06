﻿using ProjectIvy.Model.Database.Main.Inv;
using ProjectIvy.Model.Database.Main.Net;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Log
{
    [Table(nameof(BrowserLog), Schema = "Log")]
    public class BrowserLog
    {
        [Key]
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public int DomainId { get; set; }

        public DateTime TimestampEnd { get; set; }

        public DateTime TimestampStart { get; set; }

        public bool IsSecured { get; set; }

        public Device Device { get; set; }

        public Domain Domain { get; set; }
    }
}
