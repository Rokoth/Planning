using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Planning.DB.Context
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("project", "project");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).HasColumnName("name");
            builder.Property(s => s.Path).HasColumnName("path");
            builder.Property(s => s.ParentId).HasColumnName("parent_id");
            builder.Property(s => s.IsLeaf).HasColumnName("is_project");
            builder.Property(s => s.IsDeleted).HasColumnName("is_deleted");
            builder.Property(s => s.LastUsedDate).HasColumnName("last_used_date");
            builder.Property(s => s.VersionDate).HasColumnName("version_date");
            builder.Property(s => s.Period).HasColumnName("period");
            builder.Property(s => s.Priority).HasColumnName("priority");
        }
    }
}
