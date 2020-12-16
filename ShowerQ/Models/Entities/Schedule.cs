using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowerQ.Models.Entities
{
    public class Schedule
    {
        private readonly ILazyLoader _lazyLoader;

        private ICollection<Interval> _intervals;

        public Schedule()
        {
        }

        public Schedule(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        public int Id { get; set; }

        public virtual ICollection<Interval> Intervals
        {
            get => _lazyLoader.Load(this, ref _intervals);
            set => _intervals = value;
        }

        public int TenantsPerInterval { get; set; }
    }
}
