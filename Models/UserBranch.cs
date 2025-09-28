using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyApi.Models
{
    public class UserBranch : BaseEntity
    {
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        [ForeignKey("BranchId")]
        public virtual Branch? Branch { get; set; }
    }
}
