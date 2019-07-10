using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLHotChoco.Models
{
    public class UserRoles
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int RoleId { get; set; }
    }

    public class UserRolesConfig : IEntityTypeConfiguration<UserRoles>
    {
        public void Configure(EntityTypeBuilder<UserRoles> builder)
        {
            builder.ToTable("UserRoles");
            builder.HasKey(userRoles => new { userRoles.UserId, userRoles.RoleId });

            builder.HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId)
                .IsRequired();
        }
    }
}
