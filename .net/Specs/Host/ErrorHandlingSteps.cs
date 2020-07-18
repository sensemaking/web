using System;
using System.Serialization;
using Sensemaking.Bdd;
using Sensemaking.Monitoring;

namespace Sensemaking.Host.Web.Specs
{
    public partial class ErrorHandlingSpecs
    {
        private static readonly ServiceAvailabilityException service_availability_exception = new ServiceAvailabilityException();

        private void a_(Exception exception)
        {
            startup.CauseException(exception);
        }
    }

    public class ErrorStartup : Startup
    {
        private Exception exception;

        public void CauseException(Exception exception)
        {
            this.exception = exception;
        }
    }
}