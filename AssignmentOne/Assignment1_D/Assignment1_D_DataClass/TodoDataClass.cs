
namespace Assignment1_D_DataClass
{
    public sealed class TodoDataClass
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }

        public TodoDataClass(string id, string title, string description, bool isCompleted)
        {
            Id = id;
            Title = title;
            Description = description;
            IsCompleted = isCompleted;
        }

        public override string ToString()
        {
            var status = IsCompleted ? "✓" : "○";
            var truncatedDescription = Description.Length > 50
                ? Description.Substring(0, 47) + "..."
                : Description;

            return $"{status} {Title} - {truncatedDescription}";
        }
    }
}
