namespace Application.Interfaces;

public interface IHashHelper
{
    public string CalculateHash(string data);
    public string CalculateMerkleRoot(List<string> data);
}
