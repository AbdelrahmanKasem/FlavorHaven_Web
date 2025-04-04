namespace RMSProjectAPI.Database.Entity
{
    public class ContactForm
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public bool IsPrivacyPolicyAccepted { get; set; }
    }
}
