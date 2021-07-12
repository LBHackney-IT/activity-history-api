using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityHistoryApi.V1.Domain
{
    public class AuthorDetails
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }
    }
}
