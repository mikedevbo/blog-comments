namespace Common
{
    using NServiceBus;

    public interface IEndpointInitializer
    {
        void Initialize(EndpointConfiguration endpointConfiguration, bool isSendOnlyEndpoint);
    }
}
