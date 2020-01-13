using System;

namespace SerilogHelpers.Enrichers
{
    public class UserEnrichment
    {
        public Func<string> GetUserID { get; set; }
        public Func<string> GetUserName { get; set; }
    }
}