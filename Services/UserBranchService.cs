using CompanyApi.DTOs.BranchDtos;
using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.DTOs.UserDtos;
using CompanyApi.Models;
using CompanyApi.Repositories.Interfaces;
using CompanyApi.Services.Interfaces;
using System.Linq.Expressions;

namespace CompanyApi.Services
{
    public class UserBranchService : IUserBranchService
    {
        private readonly IUnitOfWork _context;

        public UserBranchService(IUnitOfWork unitOfWork)
        {
            _context = unitOfWork;
        }

        public async Task<Result<List<UserBranchDto>>> AddBranchToUserAsync(int userId,int[] branchIds)
        {
            if( branchIds == null || branchIds.Length == 0 )
                return Result<List<UserBranchDto>>.Failure("لم يتم توفير فروع.");

            // Get all existing user-branch relations for that user
            var existingUserBranches = await _context.Repository<UserBranch>()
                .FindAsync(ub => ub.UserId == userId);

            // Filter out branches that already exist
            var newBranches = branchIds
                .Where(bid => !existingUserBranches.Any(ub => ub.BranchId == bid))
                .Select(bid => new UserBranch
                {
                    UserId = userId,
                    BranchId = bid,
                    IsActive = true
                })
                .ToList();

            if( !newBranches.Any() )
                return Result<List<UserBranchDto>>.Failure("المستخدم لديه بالفعل جميع الفروع المحددة.");

            // Add new relations
            await _context.Repository<UserBranch>().AddRangeAsync(newBranches);
            await _context.SaveChangesAsync();

            var resultDtos = newBranches.Select(nb => new UserBranchDto
            {
                UserId = nb.UserId,
                BranchId = nb.BranchId,
                IsActive = nb.IsActive
            }).ToList();

            return Result<List<UserBranchDto>>.Success(resultDtos,"تم إضافة الفروع للمستخدم بنجاح.");
        }


        public async Task<Result<bool>> RemoveBranchFromUserAsync(int userId)//, int branchId
        {
            var userBranch = await _context.Repository<UserBranch>()
                .FindAsync(ub => ub.UserId == userId); // && ub.BranchId == branchId

            if( userBranch == null || !userBranch.Any() )
                return Result<bool>.Failure("المستخدم ليس لديه أي فروع");

            _context.Repository<UserBranch>().RemoveRange(userBranch);
            await _context.SaveChangesAsync();

            return Result<bool>.Success(true, "تم حذف الفرع من المستخدم بنجاح");
        }

        public async Task<Result<bool>> SetBranchActiveStatusAsync(int userId, int branchId, bool isActive)
        {
            var userBranch = await _context.Repository<UserBranch>()
                .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BranchId == branchId);

            if (userBranch == null)
                return Result<bool>.Failure("المستخدم ليس لديه هذا الفرع");

            userBranch.IsActive = isActive;
            _context.Repository<UserBranch>().Update(userBranch);
            await _context.SaveChangesAsync();

            var status = isActive ? "مفعل" : "غير مفعل";
            return Result<bool>.Success(true, $"تم {status} الفرع للمستخدم بنجاح");
        }

        public async Task<Result<IEnumerable<BranchDto>>> GetUserBranchesAsync(int userId, bool onlyActive = true)
        {
            Expression<Func<UserBranch, bool>> predicate = ub =>
                ub.UserId == userId &&
                (!onlyActive || (ub.IsActive && ub.Branch!.IsActive));

            var branches = await _context.Repository<UserBranch>()
                .AddIncludes("Branch")
                .GetProjectedAsync(predicate, ub => new BranchDto
                {
                    Id = ub.BranchId,
                    Name = ub.Branch!.Name,
                    Code = ub.Branch.Code,
                    IsActive = ub.IsActive
                });

            return Result<IEnumerable<BranchDto>>.Success(branches, "تم استرجاع فروع المستخدم بنجاح");
        }

    }
}
