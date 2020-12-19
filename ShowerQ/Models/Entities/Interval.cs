using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace ShowerQ.Models.Entities
{
    public class Interval
    {
        private readonly ILazyLoader _lazyLoader;

        private Schedule _schedule;

        public Interval()
        {
            
        }

        public Interval(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        public int Id { get; set; }

        public DateTime Beginning { get; set; }

        public DateTime Finishing { get; set; }

        public int ScheduleId { get; set; }

        public Schedule Schedule
        {
            get => _lazyLoader.Load(this, ref _schedule);
            set => _schedule = value;
        }
    }
}
