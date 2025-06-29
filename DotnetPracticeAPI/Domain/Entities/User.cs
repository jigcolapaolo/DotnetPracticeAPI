using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        //public Guid Id { get; set; }
        //public string UserName { get; set; } = default!;
        //public string Email { get; set; } = default!;
        //public string PasswordHash { get; set; } = default!;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Product>? Products { get; set; }

    }
}
