namespace UserService.Domain.Models
{
    public class UserProfile
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}