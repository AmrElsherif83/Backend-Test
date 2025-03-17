using Backend_Test.Domain.Entities;
using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Infrastructure.Persistence.Mappings
{
    public class DriverMap : ClassMapper<Driver>
    {
        public DriverMap()
        {
            Table("Driver");
            Map(d => d.Id).Key(KeyType.Assigned); // ✅ Ensure Id is explicitly mapped as primary key
            Map(d => d.FirstName).Column("FirstName");
            Map(d => d.LastName).Column("LastName");
            Map(d => d.Email).Column("Email");
            Map(d => d.PhoneNumber).Column("PhoneNumber");
            AutoMap();
        }
    }
}
