using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Data.Models
{
    public class Conversation
    {
        public Conversation()
        {
            Messages = new HashSet<Message>();
            UsersConversations = new HashSet<UserConversation>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string Title { get; set; } = null!;

        [Required]
        public DateTime StartedAt { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<UserConversation> UsersConversations { get; set; }
    }
}
