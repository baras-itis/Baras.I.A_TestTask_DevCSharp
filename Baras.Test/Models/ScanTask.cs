namespace Baras.Test.Models
{
    public class ScanTask
    {
        public Guid Id { get; set; }
        public string RepoUrl { get; set; }

        public ScanStatus Status { get; set; }

        public DateTime LastStatusChange { get; set; }
        // string? тк результат может быть null 
        public string? Result;
    }
}
