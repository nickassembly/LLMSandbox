using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using LLMSandbox;

public class KeyVaultService
{
    private readonly SecretClient _client;

    public KeyVaultService(string vaultUri)
    {
        var credential = CredentialFactory.CreateWithDeviceCodeFallback();
        _client = new SecretClient(new Uri(vaultUri), credential);
    }

    public async Task<string?> GetSecretAsync(string secretName)
    {
        try
        {
            KeyVaultSecret secret = await _client.GetSecretAsync(secretName);
            return secret.Value;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error retrieving secret:");
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}