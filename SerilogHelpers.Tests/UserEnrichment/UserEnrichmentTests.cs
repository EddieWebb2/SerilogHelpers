using System.Collections.Generic;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.TestCorrelator;
using SerilogHelpers.Enrichers;
using SerilogHelpers.Infrustructure;
using SerilogHelpers.Tests.Setup;
using Shouldly;
using Xunit;

namespace SerilogHelpers.Tests.UserEnrichment
{
    public class UserEnrichmentTests
    {
        private readonly ILogEventSink[] _sinks;
        private readonly ILoggingConfiguration _config;

        public UserEnrichmentTests()
        {
            _config = new TestConfig();
            _sinks = new ILogEventSink[]
            {
                new TestCorrelatorSink()
            };
        }

        private static readonly List<object[]> _data = new List<object[]>
        {
            new object[] {"1234", "Dennis xxxxxx"},
            new object[] {"898989", "Eddie xxxx"},
            new object[] {"999", "Jangus (you know who you are)"},
            new object[] {"4CC84F6D-2A52-41D4-AF17-4F30AE99008C", "New user"}
        };

        public static IEnumerable<object[]> TestData => _data;

        [Theory]
        [MemberData(nameof(TestData))]
        public void User_Enricher_should_add_two_properties_with_expected_keys(string userID, string userName)
        {
            var enrichers = new ILogEventEnricher[]
            {
                new UserEnricher(new Enrichers.UserEnrichment()
                {
                    GetUserID = () => userID,
                    GetUserName = () => userName
                })
            };

            Logging.InitializeLogging(_config, enrichers, _sinks);

            using (TestCorrelator.CreateContext())
            {
                Log.Error("Test Error");
                TestCorrelator.GetLogEventsFromCurrentContext().ShouldHaveSingleItem().Properties.Keys.ShouldContain("UserName", "UserID");
            }
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void User_Enricher_should_add_the_correct_user_name_property(string userID, string userName)
        {
            var enrichers = new ILogEventEnricher[]
            {
                new UserEnricher(new Enrichers.UserEnrichment()
                {
                    GetUserID = () => userID,
                    GetUserName = () => userName
                })
            };

            Logging.InitializeLogging(_config, enrichers, _sinks);

            using (TestCorrelator.CreateContext())
            {
                Log.Error("Test Error");
                TestCorrelator.GetLogEventsFromCurrentContext().ShouldHaveSingleItem().Properties["UserName"].ToString().Replace("\"", string.Empty).ShouldBe(userName);
            }
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void User_Enricher_should_add_the_correct_user_id_property(string userID, string userName)
        {
            var enrichers = new ILogEventEnricher[]
            {
                new UserEnricher(new Enrichers.UserEnrichment()
                {
                    GetUserID = () => userID,
                    GetUserName = () => userName
                })
            };

            Logging.InitializeLogging(_config, enrichers, _sinks);

            using (TestCorrelator.CreateContext())
            {
                Log.Error("Test Error");
                TestCorrelator.GetLogEventsFromCurrentContext().ShouldHaveSingleItem().Properties["UserID"].ToString().Replace("\"", string.Empty).ShouldBe(userID);
            }
        }
    }
}
