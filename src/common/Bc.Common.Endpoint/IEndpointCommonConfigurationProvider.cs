namespace Bc.Common.Endpoint
{
    public interface IEndpointCommonConfigurationProvider
    {
        string TransportConnectionString { get; }
        
        bool IsDisableRecoverability { get; }
        
        bool IsSendHeartbeats { get; }
        
        string ServiceControlAddress { get; }
        
        bool IsSendMetrics { get; }
        
        string ServiceControlMetricsAddress { get; }
    }
}