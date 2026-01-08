namespace iIpos_core.Dto.Branch
{
    public class BranchDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public int StoreId { get; set; }
    }
}
