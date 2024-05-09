namespace ADO_NET.Models
{
    public class Career
    {
        public Career()
        {
            Items = new List<CareerItem>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public IList<CareerItem> Items { get; set; }
    }
}
