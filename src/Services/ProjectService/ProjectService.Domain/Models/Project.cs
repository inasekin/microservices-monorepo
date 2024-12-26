namespace ProjectService.Domain.Models
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
    }
}