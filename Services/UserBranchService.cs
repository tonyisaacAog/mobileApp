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
                return Result<List<UserBranchDto>>.Failure("No branches provided.");

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
                return Result<List<UserBranchDto>>.Failure("User already has all selected branches.");

            // Add new relations
            await _context.Repository<UserBranch>().AddRangeAsync(newBranches);
            await _context.SaveChangesAsync();

            var resultDtos = newBranches.Select(nb => new UserBranchDto
            {
                UserId = nb.UserId,
                BranchId = nb.BranchId,
                IsActive = nb.IsActive
            }).ToList();

            return Result<List<UserBranchDto>>.Success(resultDtos,"Branches added to user successfully.");
        }


        public async Task<Result<bool>> RemoveBranchFromUserAsync(int userId)//, int branchId
        {
            var userBranch = await _context.Repository<UserBranch>()
                .FindAsync(ub => ub.UserId == userId); // && ub.BranchId == branchId

            if( userBranch == null || !userBranch.Any() )
                return Result<bool>.Failure("User does not have any branches");

            _context.Repository<UserBranch>().RemoveRange(userBranch);
            await _context.SaveChangesAsync();

            return Result<bool>.Success(true, "Branch removed from user successfully");
        }

        public async Task<Result<bool>> SetBranchActiveStatusAsync(int userId, int branchId, bool isActive)
        {
            var userBranch = await _context.Repository<UserBranch>()
                .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BranchId == branchId);

            if (userBranch == null)
                return Result<bool>.Failure("User does not have this branch");

            userBranch.IsActive = isActive;
            _context.Repository<UserBranch>().Update(userBranch);
            await _context.SaveChangesAsync();

            var status = isActive ? "activated" : "deactivated";
            return Result<bool>.Success(true, $"Branch {status} for user successfully");
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

            return Result<IEnumerable<BranchDto>>.Success(branches, "User branches retrieved successfully");
        }

    }
}
