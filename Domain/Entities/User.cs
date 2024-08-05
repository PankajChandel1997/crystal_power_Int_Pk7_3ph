using Domain.Interface;
using Domain.Model;
using System;

namespace Domain.Entities
{
    public class User : Entity<int>, ITrackCreated, ITrackUpdated
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
