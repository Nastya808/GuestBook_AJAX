using GuestBookApp.Models;

public interface IUserRepository
{
    Task<User> GetByIdAsync(int id);
    Task<User> GetByNameAsync(string name); 
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
}
