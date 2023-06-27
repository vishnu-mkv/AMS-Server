namespace AMS.Interfaces
{
    public interface ISecurePasswordHasher
    {
        string Hash(string password, int iterations);
        bool Verify(string password, string hashedPassword);

        public string Hash(string password);

        public bool IsHashSupported(string hashString);
    }
}
