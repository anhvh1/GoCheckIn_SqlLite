using System;
using Com.Gosol.INOUT.Ultilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Com.Gosol.INOUT.DAL.EFCore
{
    public partial class DbINOUTContext : DbContext
    {
        public DbINOUTContext()
        {
        }

        public DbINOUTContext(DbContextOptions<DbINOUTContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DanhMucCanBo> DanhMucCanBo { get; set; }
        public virtual DbSet<DanhMucChucVu> DanhMucChucVu { get; set;  }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //optionsBuilder.UseMySql(SQLHelper.appConnectionStrings);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DanhMucCanBo>(entity =>
            {
                entity.HasKey(e => e.CanBoId)
                    .HasName("PRIMARY");

                entity.ToTable("DanhMuc_can_bo");

                entity.HasIndex(e => e.ChucVuId)
                    .HasName("ChucVuID");

                entity.HasIndex(e => e.CoQuanCuId)
                    .HasName("canbo_coquan_idx");

                entity.Property(e => e.CanBoId)
                    .HasColumnName("CanBoID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.CanBoCuId)
                    .HasColumnName("CanBoCuID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.ChucVuId)
                    .HasColumnName("ChucVuID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.CoQuanCuId)
                    .HasColumnName("CoQuanCuID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.CoQuanId)
                    .HasColumnName("CoQuanID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.DiaChi).HasColumnType("NVarChar(255)");

                entity.Property(e => e.DienThoai).HasColumnType("NVarChar(20)");

                entity.Property(e => e.Email).HasColumnType("NVarChar(100)");

                entity.Property(e => e.GioiTinh).HasColumnType("tinyint(1)");

                entity.Property(e => e.IsStatus).HasColumnType("bit(1)");

                entity.Property(e => e.NgaySinh).HasColumnType("datetime");

                entity.Property(e => e.PhongBanId)
                    .HasColumnName("PhongBanID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.QuanTridonVi).HasColumnType("int(11)");

                entity.Property(e => e.QuyenKy).HasColumnType("tinyint(1)");

                entity.Property(e => e.RoleId)
                    .HasColumnName("RoleID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.TenCanBo).HasColumnType("NVarChar(50)");

                entity.Property(e => e.XemTaiLieuMat).HasColumnType("tinyint(1)");

                entity.HasOne(d => d.ChucVu)
                    .WithMany(p => p.DanhMucCanBo)
                    .HasForeignKey(d => d.ChucVuId)
                    .HasConstraintName("DanhMuc_can_bo_ibfk_1");
            });

            modelBuilder.Entity<DanhMucChucVu>(entity =>
            {
                entity.HasKey(e => e.ChucVuId)
                    .HasName("PRIMARY");

                entity.ToTable("DanhMuc_chuc_vu");

                entity.Property(e => e.ChucVuId)
                    .HasColumnName("ChucVuID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.GhiChu).HasColumnType("NVarChar(50)");

                entity.Property(e => e.TenChucVu).HasColumnType("NVarChar(50)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
