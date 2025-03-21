using Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Application.Common.Services;

public class HashHelper : IHashHelper
{
    public string CalculateHash(string data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        byte[] hash = SHA3_512.HashData(bytes);
        return Encoding.UTF8.GetString(hash);
    }
}
