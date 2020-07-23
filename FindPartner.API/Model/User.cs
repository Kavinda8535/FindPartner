namespace FindPartner.API.Model
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public byte[] PassowrdHash { get; set; }

        public byte[] PasswordSalt { get; set; }
    }
}