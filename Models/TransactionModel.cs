using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerAPI.Models
{
    public class TransactionModel
    {
        public int TransactionID { get; set; }
        public string? Note { get; set; }
        public int UserID { get; set; }
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public string? Type { get; set; }
        public string? Icon { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
    public class CategoryDropDownModel
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Icon { get; set; } 
    }
    public class TransactionFilterModel
    {
        public int UserID { get; set; }
        public int? CategoryID { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string? Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
