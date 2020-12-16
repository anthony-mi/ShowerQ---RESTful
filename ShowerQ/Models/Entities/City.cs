using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowerQ.Models.Entities
{
    public class City
    {
        private readonly ILazyLoader _lazyLoader;
        private ICollection<University> _universities;

        public City()
        {
        }

        public City(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<University> Universities
        {
            get => _lazyLoader.Load(this, ref _universities);
            set => _universities = value;
        }
    }
}
