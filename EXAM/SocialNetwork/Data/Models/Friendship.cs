using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Data.Models
{
    public class Friendship
    {
        public int UserOneId { get; set; }

        [ForeignKey(nameof(UserOneId))]
        public virtual User UserOne { get; set; } = null!;

        public int UserTwoId { get; set; }

        [ForeignKey(nameof(UserTwoId))]
        public virtual User UserTwo { get; set; } = null!;
    }
}
