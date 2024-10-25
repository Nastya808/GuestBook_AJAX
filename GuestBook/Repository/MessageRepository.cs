using GuestBookApp.Data;
using GuestBookApp.Models;
using Microsoft.EntityFrameworkCore;

public class MessageRepository : IRepository<Message>
{
    private readonly GuestBookContext _context;

    public MessageRepository(GuestBookContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Message>> GetAllAsync()
    {
        return await _context.Messages.ToListAsync();
    }

    public async Task<Message> GetByIdAsync(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task AddAsync(Message entity)
    {
        await _context.Messages.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Message entity)
    {
        _context.Messages.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var message = await GetByIdAsync(id);
        if (message != null)
        {
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }
    }
}