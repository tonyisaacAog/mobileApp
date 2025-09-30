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

        public async Task<Result<UserBranchDto>> AddBranchToUserAsync(int userId, int branchId)
        {
            var exists = await _context.Repository<UserBranch>().FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BranchId == branchId);

            if (exists != null)
            {
                if (!exists.IsActive)
                {
                    exists.IsActive = true;
                    _context.Repository<UserBranch>().Update(exists);
                    await _context.SaveChangesAsync();

                    return Result<UserBranchDto>.Success(new UserBranchDto
                    {
                        UserId = exists.UserId,
                        BranchId = exists.BranchId,
                        IsActive = exists.IsActive
                    }, "Branch reactivated for user");
                }

                return Result<UserBranchDto>.Failure("User already has this branch");
            }

            var userBranch = new UserBranch
            {
                UserId = userId,
                BranchId = branchId,
                IsActive = true
            };

            await _context.Repository<UserBranch>().AddAsync(userBranch);
            await _context.SaveChangesAsync();

            return Result<UserBranchDto>.Success(new UserBranchDto
            {
                UserId = userId,
                BranchId = branchId,
                IsActive = true
            }, "Branch added to user successfully");
        }

        public async Task<Result<bool>> RemoveBranchFromUserAsync(int userId, int branchId)
        {
            var userBranch = await _context.Repository<UserBranch>()
                .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BranchId == branchId);

            if (userBranch == null)
                return Result<bool>.Failure("User does not have this branch");

            _context.Repository<UserBranch>().Remove(userBranch);
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
