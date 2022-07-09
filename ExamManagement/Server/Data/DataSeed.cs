using ExamManagement.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExamManagement.Server.Data
{
    public static class DataSeed
    {
        public static ModelBuilder SeedAllData(this ModelBuilder builder) 
        {
            builder.Entity<ItemTypeCategory>()
                .HasData(new ItemTypeCategory 
                {
                    Id = 1,
                    CategoryName = "Semester",
                    NormalizedCategoryName = "SEMESTER" 
                });

            builder.Entity<ItemType>()
                .HasData(new ItemType
                {
                    Id = 1,
                    Name = "FIRST SEMESTER",
                    ItemTypeCategoryId = 1
                });

            builder.Entity<ItemType>()
                .HasData(new ItemType
                {
                    Id = 2,
                    Name = "SECOND SEMESTER",
                    ItemTypeCategoryId = 1
                });

            builder.Entity<ItemType>()
                .HasData(new ItemType
                {
                    Id = 3,
                    Name = "THIRD SEMESTER",
                    ItemTypeCategoryId = 1
                });

            builder.Entity<ItemType>()
                .HasData(new ItemType
                {
                    Id = 4,
                    Name = "FOURTH SEMESTER",
                    ItemTypeCategoryId = 1
                });

            builder.Entity<ItemType>()
                .HasData(new ItemType
                {
                    Id = 5,
                    Name = "FIFTH SEMESTER",
                    ItemTypeCategoryId = 1
                });

            builder.Entity<ItemType>()
                .HasData(new ItemType
                {
                    Id = 6,
                    Name = "SIXTH SEMESTER",
                    ItemTypeCategoryId = 1
                });

            builder.Entity<ItemType>()
                .HasData(new ItemType
                {
                    Id = 7,
                    Name = "SEVENTH SEMESTER",
                    ItemTypeCategoryId = 1
                });

            builder.Entity<ItemType>()
                .HasData(new ItemType
                {
                    Id = 8,
                    Name = "EIGHTH SEMESTER",
                    ItemTypeCategoryId = 1
                });

            return builder;
        }
    }
}
