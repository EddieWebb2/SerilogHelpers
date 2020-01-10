using Serilog.Core;
using Serilog.Events;

namespace SerilogHelpers.Enrichers
{
    public class UserEnricher : ILogEventEnricher
    {
        private readonly UserEnrichment _enricher;

        public UserEnricher(UserEnrichment enricher)
        {
            _enricher = enricher;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddOrUpdateProperty(new LogEventProperty("UserName", new ScalarValue(_enricher.GetUserName())));
            logEvent.AddOrUpdateProperty(new LogEventProperty("UserID", new ScalarValue(_enricher.GetUserID())));
        }
    }
}
