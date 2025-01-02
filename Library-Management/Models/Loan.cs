public class Loan
{
    public string Id { get; set; }
    public string BookId { get; set; }
    public string Username { get; set; }
    public DateTime BorrowedDate { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsReturned { get; set; }
    public DateTime? ReturnedDate { get; set; }
}