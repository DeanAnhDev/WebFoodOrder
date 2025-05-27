namespace FoodOrder.Application.Common.Models
{
    public class EmailDto
    {
        public List<string> To { get; set; } = new();
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public EmailDto(List<string> to, string subject, string content)
        {
            To = to;
            Subject = subject;
            Content = content;
        }

    }
}
