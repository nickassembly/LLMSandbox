using Azure.Core;
using Azure.Identity;

namespace LLMSandbox
{
    public class CredentialFactory
    {
        public static TokenCredential CreateWithDeviceCodeFallback()
        {
            try
            {
                return new DefaultAzureCredential();
            }
            catch
            {
                // Fallback to DeviceCodeCredential when default fails
                return new DeviceCodeCredential(new DeviceCodeCredentialOptions
                {
                    DeviceCodeCallback = (DeviceCodeInfo info, System.Threading.CancellationToken cancellationToken) =>
                    {
                        Console.WriteLine(info.Message);
                        return Task.CompletedTask;
                    }
                });
            }
        }
    }
}
