using ProjectIvy.Common.Extensions;
using System;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Call
{
    public class Call
    {
        public Call(DatabaseModel.User.Call c)
        {
            Duration = c.Duration;
            File = c.File.ConvertTo(x => new File.File(x));
            Number = c.Number;
            Timestamp = c.Timestamp;
        }

        public DateTime Timestamp { get; set; }

        public string Number { get; set; }

        public int Duration { get; set; }

        public File.File File { get; set; }
    }
}
