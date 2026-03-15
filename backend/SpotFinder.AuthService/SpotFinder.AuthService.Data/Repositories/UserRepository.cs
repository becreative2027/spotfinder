using Microsoft.EntityFrameworkCore;
using SpotFinder.AuthService.Data.Context;
using SpotFinder.AuthService.Data.Entities;

namespace SpotFinder.AuthService.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower(), ct);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        => await _context.Users.AnyAsync(u => u.Email == email.ToLower(), ct);

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        user.Email = user.Email.ToLower();
        await _context.Users.AddAsync(user, ct);
    }

    public Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
