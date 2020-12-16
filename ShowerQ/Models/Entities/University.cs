using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowerQ.Models.Entities
{
    public class University
    {
        private readonly ILazyLoader _lazyLoader;

        private City _city;
        private ICollection<Dormitory> _dormitories;

        public University()
        {
        }

        public University(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int CityId { get; set; }

        public virtual City City
        {
            get => _lazyLoader.Load(this, ref _city);
            set => _city = value;
        }

        public virtual ICollection<Dormitory> Dormitories
        {
            get => _lazyLoader.Load(this, ref _dormitories);
            set => _dormitories = value;
        }
    }
}
