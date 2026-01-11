using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Data.Models
{
    public class User
    {
        public User()
        {
            Posts = new HashSet<Post>();
            Messages = new HashSet<Message>();
            UsersConversations = new HashSet<UserConversation>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 4)]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(60, MinimumLength = 8)]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        public virtual ICollection<Post> Posts { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<UserConversation> UsersConversations { get; set; }
    }
}
