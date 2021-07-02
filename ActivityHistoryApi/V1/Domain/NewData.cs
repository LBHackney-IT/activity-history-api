using System;

namespace ActivityHistoryApi.V1.Domain
{
    public class NewData
    {
        public Guid Id { get; set; }
        public Title Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }

    }
}
