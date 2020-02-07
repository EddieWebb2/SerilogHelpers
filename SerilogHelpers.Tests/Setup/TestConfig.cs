using Serilog.Events;
using SerilogHelpers.Infrustructure;
using System;

namespace SerilogHelpers.Tests.Setup
{
    public class TestConfig : ILoggingConfiguration
    {
        public Uri LoggingEndpoint { get; set; }
        public bool EnableConsoleLogging { get; }
        public bool EnableElasticLogging { get; set; }
        public bool WriteToTempPath { get; set; }
        public string SoftwareName { get; set; }
        public LogEventLevel ElasticLoggingLevel { get; set; }

        public TestConfig()
        {
            LoggingEndpoint = new Uri("http://localhost:9200");
            EnableElasticLogging = false;
            EnableConsoleLogging = false;
            SoftwareName = "KibanaTester";
            ElasticLoggingLevel = LogEventLevel.Debug;
            WriteToTempPath = false;
        }
    }
}