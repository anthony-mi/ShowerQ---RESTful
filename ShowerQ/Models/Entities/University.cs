using System;
using System.Collections.Generic;
using System.Text;

namespace ShowerQ.Models.Entities
{
    public class University
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public virtual City City { get; set; }
    }
}
