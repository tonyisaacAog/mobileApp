using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyApi.Models
{
    public class Device : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Serial { get; set; } = string.Empty;
        public string OS { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }
    }
}
