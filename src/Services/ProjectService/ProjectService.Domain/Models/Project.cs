namespace ProjectService.Domain.Models
{
    public class Project
    {
        public Guid Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
    }
}