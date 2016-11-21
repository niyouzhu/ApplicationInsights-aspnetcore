namespace Microsoft.ApplicationInsights.AspNetCore
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.ApplicationInsights.AspNetCore.DiagnosticListeners;
    using Microsoft.ApplicationInsights.AspNetCore.Extensions;
    using Microsoft.ApplicationInsights.AspNetCore.Logging;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    internal class ApplicationInsightsInitializer : IObserver<DiagnosticListener>, IDisposable
    {
        private readonly List<IDisposable> subscriptions;
        private readonly IEnumerable<IApplicationInsightDiagnosticListener> diagnosticListeners;

        public ApplicationInsightsInitializer(
            IEnumerable<IApplicationInsightDiagnosticListener> diagnosticListeners,
            IOptions<ApplicationInsightsServiceOptions> options,
            TelemetryClient telemetryClient,
            ILoggerFactory loggerFactory)
        {
            this.diagnosticListeners = diagnosticListeners;
            this.subscriptions = new List<IDisposable>();

            loggerFactory.AddProvider(new ApplicationInsightsLoggerProvider(telemetryClient, options.Value.LoggerMinimumLevel));
        }

        public void Start()
        {
            DiagnosticListener.AllListeners.Subscribe(this);
        }

        void IObserver<DiagnosticListener>.OnNext(DiagnosticListener value)
        {
            foreach (var applicationInsightDiagnosticListener in this.diagnosticListeners)
            {
                if (applicationInsightDiagnosticListener.ListenerName == value.Name)
                {
                    this.subscriptions.Add(value.SubscribeWithAdapter(applicationInsightDiagnosticListener));
                }
            }
        }

        void IObserver<DiagnosticListener>.OnError(Exception error)
        {
        }

        void IObserver<DiagnosticListener>.OnCompleted()
        {
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var subscription in this.subscriptions)
                {
                    subscription.Dispose();
                }
            }
        }
    }
}