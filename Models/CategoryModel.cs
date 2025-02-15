namespace ExpenseTrackerAPI.Models
{
    public class CategoryModel
    {
        public int? CategoryID { get; set; }
        public int UserID { get; set; }
        public string CategoryName { get; set; }
        public string Icon { get; set; }
        public string Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
