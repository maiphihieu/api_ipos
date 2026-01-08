namespace iIpos_core.Dto.TableInfo
{
    public class TableInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int BranchId { get; set; }
        public int? StoreId { get; set; }
        public string Token { get; set; } = null!;
    }
}
