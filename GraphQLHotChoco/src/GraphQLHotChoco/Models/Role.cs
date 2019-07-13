using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace GraphQLHotChoco.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<UserRoles> RoleUsers { get; set; }
    }

    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(role => role.Id);

            builder.HasMany(x => x.RoleUsers)
                .WithOne(x => x.Role)
                .HasForeignKey(x => x.RoleId)
                .IsRequired();
        }
    }

    public class RoleObjectType : ObjectType<Role>
    {
        protected override void Configure(IObjectTypeDescriptor<Role> descriptor)
        {
            descriptor.Field(user => user.Id).Type<IntType>();
            descriptor.Field(user => user.Name).Type<StringType>();

            descriptor.Field(user => user.RoleUsers);
        }
    }
}
