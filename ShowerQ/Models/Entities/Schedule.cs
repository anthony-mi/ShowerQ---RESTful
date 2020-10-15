using System;
using System.Collections.Generic;
using System.Text;

namespace ShowerQ.Models.Entities
{
    public class Schedule
    {
        public int Id { get; set; }

        public virtual IEnumerable<Interval> Intervals { get; set; }

        public int TenantsPerInterval { get; set; }
    }
}
