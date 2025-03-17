using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Domain.Common.Exceptions
{
   
    public class NotFoundException : Exception
    {
        public NotFoundException(string entityName, object key)
            : base($"'{entityName}' ({key}) was not found.")
        {
        }
    }

}
