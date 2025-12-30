using Microsoft.EntityFrameworkCore;

namespace TodoApi;

public class ToDoDbContext : DbContext
{
    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options)
    {
    }

    public DbSet<Item> Items { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // החליפי את '1234' בסיסמה האמיתית שלך ל-MySQL
            optionsBuilder.UseMySql("server=localhost;user=root;password=0534125960$;database=tododb", 
                Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.0-mysql"));
        }
    }
}

// הגדרת המחלקות כאן פעם אחת בלבד בכל הפרויקט
public class User 
{ 
    public int Id { get; set; } 
    public string Username { get; set; } = string.Empty; 
    public string Password { get; set; } = string.Empty; 
}

public class Item
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
    public int UserId { get; set; } 
}