using System;
using System.Collections.Generic;
using System.Text;

namespace ShowerQ.Models.Entities
{
    public class Interval
    {
        public int Id { get; set; }

        public DateTime Beginning { get; set; }

        public DateTime Finishing { get; set; }
    }
}
