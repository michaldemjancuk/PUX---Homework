using System.Security.Cryptography;

namespace PuxHomework.Services;

public interface IMd5CalculatorService
{
    public string CalculateMD5(string fileName);
}

public class Md5CalculatorService : IMd5CalculatorService
{
    public string CalculateMD5(string fileName)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(fileName))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
