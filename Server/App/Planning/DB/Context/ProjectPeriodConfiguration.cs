using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB.Context
{
    public class ProjectPeriodConfiguration : IEntityTypeConfiguration<ProjectPeriod>
    {
        public void Configure(EntityTypeBuilder<ProjectPeriod> builder)
        {
            builder.ToTable("project_period", "project");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Order).HasColumnName("order");
            builder.Property(s => s.ProjectId).HasColumnName("project_id");
            builder.Property(s => s.DateBegin).HasColumnName("date_begin");
            builder.Property(s => s.DateEnd).HasColumnName("date_end");
            builder.Property(s => s.IsClosed).HasColumnName("is_closed");
            builder.Property(s => s.IsDeleted).HasColumnName("is_deleted");
            builder.Ignore(s => s.Project);
        }
    }
}
