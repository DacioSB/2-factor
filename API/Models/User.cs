namespace Models
{
    public class User
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public long Id { get; set; }
        public string? ActivationKey { get; set; }
        public int Active { get; set; }
        public int SignedIn { get; set; }
        public int SignedInValidated { get; set; }

    }
}