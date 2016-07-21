using System;

namespace PoC.Sqs.Core.Adapters.DataStore.DocumentDb
{
    public class DocumentDbHealthChecker : IDependencyHealthCheck
    {
        public bool IsHealthy
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}