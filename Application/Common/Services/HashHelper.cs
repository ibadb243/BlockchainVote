using Application.Interfaces;
using System.Collections.Generic;
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

    public string CalculateMerkleRoot(List<string> data)
    {
        if (data == null || data.Count == 0) throw new ArgumentException("Hashes list cannot be empty");

        if (data.Count == 1)
            return data[0];

        List<string> currentLevel = new List<string>(data);

        while (currentLevel.Count > 1)
        {
            if (currentLevel.Count % 2 == 1)
                currentLevel.Add(currentLevel.Last());

            List<string> nextLevel = new List<string>();

            for (int i = 0; i < currentLevel.Count; i += 2)
            {
                string combinedHash = CalculateHash(currentLevel[i] + currentLevel[i + 1]);
                nextLevel.Add(combinedHash);
            }

            currentLevel = nextLevel;
        }

        return currentLevel[0];
    }
}
