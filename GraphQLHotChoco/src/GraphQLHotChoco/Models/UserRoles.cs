using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphQLHotChoco.Models
{
    public class UserRoles
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
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

    public class UserRolesObjectType : ObjectType<UserRoles>
    {
        protected override void Configure(IObjectTypeDescriptor<UserRoles> descriptor)
        {
            descriptor.Field(user => user.RoleId).Type<IntType>();
            descriptor.Field(user => user.UserId).Type<IntType>();

            descriptor.Field(user => user.User);
            descriptor.Field(user => user.Role);
        }
    }
}
