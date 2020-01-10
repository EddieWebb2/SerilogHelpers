using System;
using System.IO;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Elasticsearch;
using SerilogHelpers.Infrustructure;

namespace SerilogHelpers
{
    public class Logging
    {
        private const string DefaultOutputTemplate = "{Timestamp} {SourceContext:l}: [{Level}] {Message}{NewLine}{Exception}";

        public static void InitializeLogging(ILoggingConfiguration config)
        {
            InitializeLogging(config, null, null);
        }

        public static void InitializeLogging(ILoggingConfiguration config, ILogEventEnricher[] enrichers, ILogEventSink[] sinks)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var logPath = Path.Combine(baseDirectory, "logs");

            // Use local temp path for Desktop Applications
            if (config.WriteToTempPath)
                logPath = Path.Combine(Path.GetTempPath(), "gatlogs");

            var logConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithProperty("softwareName", config.SoftwareName)
                .Enrich.FromLogContext();

            if (enrichers != null)
                logConfig.Enrich.With(enrichers);

            logConfig.WriteTo.RollingFile(Path.Combine(logPath, $"{config.SoftwareName}.log"),
                                    outputTemplate: DefaultOutputTemplate);

            if (config.EnableConsoleLogging)
                logConfig.WriteTo.Console(outputTemplate: DefaultOutputTemplate);

            if (config.EnableElasticLogging)
            {
                logConfig.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(config.LoggingEndpoint)
                {
                    MinimumLogEventLevel = config.ElasticLoggingLevel,
                    AutoRegisterTemplate = true,
                });
            }

            if (sinks != null)
                foreach (var sink in sinks)
                {
                    logConfig.WriteTo.Sink(sink);
                }

            Log.Logger = logConfig.CreateLogger();

            Log.ForContext<Logging>()
               .Information("Configuration: {@config}", config);

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                Log.ForContext<Logging>().Error(ex, ex?.Message);
            };
        }

    }
}
