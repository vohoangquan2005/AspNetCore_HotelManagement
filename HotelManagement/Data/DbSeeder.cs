using HotelManagement.Models;
using HotelManagement.Enums;

namespace HotelManagement.Data
{
    public class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
// =============================================== ROLE ===============================================
            if (!context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role
                    {
                        RoleName = "Admin",
                        Description = "Quản trị hệ thống"
                    },
                    new Role
                    {
                        RoleName = "Staff",
                        Description = "Nhân viên khách sạn"
                    },
                    new Role
                    {
                        RoleName = "Customer",
                        Description = "Khách hàng"
                    }
                };
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }
// =============================================== USER ===============================================
            if (!context.Users.Any())
            {
                var adminRole = context.Roles
                    .First(r => r.RoleName == "Admin");
                var staffRole = context.Roles
                    .First(r => r.RoleName == "Staff");
                var customerRole = context.Roles
                    .First(r => r.RoleName == "Customer");
                var users = new List<User>
                {
                    new User
                    {
                        FullName = "Nguyễn Văn Admin",
                        Email = "admin@hotel.com",
                        HashPassword = "123456",
                        RoleId = adminRole.RoleId,
                        IsLocked = ActiveEnums.Active,
                        Gender = "Nam",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        PhoneNumber = "0900000001",
                        Avatar = null,
                        IsDeleted = ActiveEnums.Active
                    },
                    new User
                    {
                        FullName = "Trần Văn Nhân",
                        Email = "staff@hotel.com",
                        HashPassword = "123456",
                        RoleId = staffRole.RoleId,
                        IsLocked = ActiveEnums.Active,
                        Gender = "Nam",
                        DateOfBirth = new DateTime(1995, 5, 10),
                        PhoneNumber = "0900000002",
                        Avatar = null,
                        IsDeleted = ActiveEnums.Active
                    },
                    new User
                    {
                        FullName = "Nguyễn Văn An",
                        Email = "an@gmail.com",
                        HashPassword = "123456",
                        RoleId = customerRole.RoleId,
                        IsLocked = ActiveEnums.Active,
                        Gender = "Nam",
                        DateOfBirth = new DateTime(2000, 3, 15),
                        PhoneNumber = "0900000003",
                        Avatar = null,
                        IsDeleted = ActiveEnums.Active
                    },
                    new User
                    {
                        FullName = "Trần Thị Bình",
                        Email = "binh@gmail.com",
                        HashPassword = "123456",
                        RoleId = customerRole.RoleId,
                        IsLocked = ActiveEnums.Active,
                        Gender = "Nữ",
                        DateOfBirth = new DateTime(1999, 8, 20),
                        PhoneNumber = "0900000004",
                        Avatar = null,
                        IsDeleted = ActiveEnums.Active
                    },
                    new User
                    {
                        FullName = "Lê Minh Cường",
                        Email = "cuong@gmail.com",
                        HashPassword = "123456",
                        RoleId = customerRole.RoleId,
                        IsLocked = ActiveEnums.Active,
                        Gender = "Nam",
                        DateOfBirth = new DateTime(2001, 12, 5),
                        PhoneNumber = "0900000005",
                        Avatar = null,
                        IsDeleted = ActiveEnums.Active
                    }
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }
// =============================================== CUSTOMER ===============================================
            if (!context.Customers.Any())
            {
                var userAn = context.Users
                    .First(u => u.Email == "an@gmail.com");
                var userBinh = context.Users
                    .First(u => u.Email == "binh@gmail.com");
                var userCuong = context.Users
                    .First(u => u.Email == "cuong@gmail.com");
                var customers = new List<Customer>
                {
                    new Customer
                    {
                        UserId = userAn.UserId,
                        CCCD = "079200000001",
                        Nationality = "Việt Nam"
                    },
                    new Customer
                    {
                        UserId = userBinh.UserId,
                        CCCD = "079200000002",
                        Nationality = "Việt Nam"
                    },
                    new Customer
                    {
                        UserId = userCuong.UserId,
                        CCCD = "079200000003",
                        Nationality = "Việt Nam"
                    }
                };
                context.Customers.AddRange(customers);
                context.SaveChanges();
            }
// =============================================== ROOM TYPE ===============================================
            if (!context.RoomTypes.Any())
            {
                var roomTypes = new List<RoomType>
                {
                    new RoomType
                    {
                        RoomTypeName = "Standard",
                        BedCount = 1,
                        BedType = "Giường đơn",
                        Size = "5m x 4m",
                        Description = "Phòng tiêu chuẩn dành cho 1-2 người."
                    },
                    new RoomType
                    {
                        RoomTypeName = "Deluxe",
                        BedCount = 1,
                        BedType = "Giường đôi",
                        Size = "6m x 5m",
                        Description = "Phòng Deluxe rộng rãi, phù hợp cho 2 người."
                    },
                    new RoomType
                    {
                        RoomTypeName = "Family",
                        BedCount = 2,
                        BedType = "2 giường đôi",
                        Size = "8m x 6m",
                        Description = "Phòng gia đình dành cho tối đa 4 người."
                    },
                    new RoomType
                    {
                        RoomTypeName = "Suite",
                        BedCount = 2,
                        BedType = "Giường đôi",
                        Size = "10m x 6m",
                        Description = "Phòng Suite cao cấp với không gian rộng."
                    }
                };
                context.RoomTypes.AddRange(roomTypes);
                context.SaveChanges();
            }
// =============================================== ROOM ===============================================
            if (!context.Rooms.Any())
            {
                var standard = context.RoomTypes
                    .First(rt => rt.RoomTypeName == "Standard");
                var deluxe = context.RoomTypes
                    .First(rt => rt.RoomTypeName == "Deluxe");
                var family = context.RoomTypes
                    .First(rt => rt.RoomTypeName == "Family");
                var suite = context.RoomTypes
                    .First(rt => rt.RoomTypeName == "Suite");
                var rooms = new List<Room>
                {
                    // Standard
                    new Room
                    {
                        RoomName = "101",
                        RoomTypeId = standard.RoomTypeId,
                        Capacity = 2,
                        Location = "Tầng 1",
                        PricePerNight = 300000,
                        Description = "Phòng Standard tầng 1.",
                        Status = RoomEnums.Available
                    },
                    new Room
                    {
                        RoomName = "102",
                        RoomTypeId = standard.RoomTypeId,
                        Capacity = 2,
                        Location = "Tầng 1",
                        PricePerNight = 300000,
                        Description = "Phòng Standard tầng 1.",
                        Status = RoomEnums.Available
                    },
                    // Deluxe
                    new Room
                    {
                        RoomName = "201",
                        RoomTypeId = deluxe.RoomTypeId,
                        Capacity = 2,
                        Location = "Tầng 2",
                        PricePerNight = 500000,
                        Description = "Phòng Deluxe tầng 2.",
                        Status = RoomEnums.Available
                    },
                    new Room
                    {
                        RoomName = "202",
                        RoomTypeId = deluxe.RoomTypeId,
                        Capacity = 2,
                        Location = "Tầng 2",
                        PricePerNight = 500000,
                        Description = "Phòng Deluxe tầng 2.",
                        Status = RoomEnums.Available
                    },
                    // Family
                    new Room
                    {
                        RoomName = "301",
                        RoomTypeId = family.RoomTypeId,
                        Capacity = 4,
                        Location = "Tầng 3",
                        PricePerNight = 800000,
                        Description = "Phòng Family dành cho gia đình.",
                        Status = RoomEnums.Available
                    },
                    new Room
                    {
                        RoomName = "302",
                        RoomTypeId = family.RoomTypeId,
                        Capacity = 4,
                        Location = "Tầng 3",
                        PricePerNight = 800000,
                        Description = "Phòng Family dành cho gia đình.",
                        Status = RoomEnums.Available
                    },
                    // Suite
                    new Room
                    {
                        RoomName = "401",
                        RoomTypeId = suite.RoomTypeId,
                        Capacity = 4,
                        Location = "Tầng 4",
                        PricePerNight = 1200000,
                        Description = "Phòng Suite cao cấp.",
                        Status = RoomEnums.Available
                    }
                };

                context.Rooms.AddRange(rooms);
                context.SaveChanges();
            }
// =============================================== ROOM IMAGES ===============================================

// ===============================================SERVICE ===============================================
            if (!context.Services.Any())
            {
                var services = new List<Service>
                {
                    new Service
                    {
                        ServiceName = "Nước suối",
                        UnitPrice = 10000,
                        Unit = "Chai",
                        Description = "Nước suối đóng chai.",
                        IsDeleted = ActiveEnums.Active
                    },
                    new Service
                    {
                        ServiceName = "Nước ngọt",
                        UnitPrice = 15000,
                        Unit = "Lon",
                        Description = "Các loại nước ngọt đóng lon.",
                        IsDeleted = ActiveEnums.Active
                    },
                    new Service
                    {
                        ServiceName = "Giặt ủi",
                        UnitPrice = 50000,
                        Unit = "Kg",
                        Description = "Dịch vụ giặt và ủi quần áo.",
                        IsDeleted = ActiveEnums.Active
                    },
                    new Service
                    {
                        ServiceName = "Ăn sáng",
                        UnitPrice = 80000,
                        Unit = "Suất",
                        Description = "Suất ăn sáng tại khách sạn.",
                        IsDeleted = ActiveEnums.Active
                    },
                    new Service
                    {
                        ServiceName = "Thuê xe máy",
                        UnitPrice = 150000,
                        Unit = "Ngày",
                        Description = "Dịch vụ cho thuê xe máy.",
                        IsDeleted = ActiveEnums.Active
                    },
                    new Service
                    {
                        ServiceName = "Đưa đón sân bay",
                        UnitPrice = 300000,
                        Unit = "Chuyến",
                        Description = "Dịch vụ đưa đón khách từ sân bay.",
                        IsDeleted = ActiveEnums.Active
                    }
                };
                context.Services.AddRange(services);
                context.SaveChanges();
            }
// =============================================== SERVICE PACKAGE ===============================================
            if (!context.ServicePackages.Any())
            {
                var servicePackages = new List<ServicePackage>
                {
                    new ServicePackage
                    {
                        PackageName = "Gói ăn sáng",
                        PackagePrice = 150000,
                        Description = "Bao gồm bữa sáng cho khách trong thời gian lưu trú.",
                        IsDeleted = ActiveEnums.Active
                    },
                    new ServicePackage
                    {
                        PackageName = "Gói tiện ích",
                        PackagePrice = 250000,
                        Description = "Bao gồm nước suối, nước ngọt và ăn sáng.",
                        IsDeleted = ActiveEnums.Active
                    },
                    new ServicePackage
                    {
                        PackageName = "Gói gia đình",
                        PackagePrice = 400000,
                        Description = "Bao gồm ăn sáng, nước uống và một số tiện ích dành cho gia đình.",
                        IsDeleted = ActiveEnums.Active
                    },
                    new ServicePackage
                    {
                        PackageName = "Gói cao cấp",
                        PackagePrice = 600000,
                        Description = "Bao gồm ăn sáng, nước uống, giặt ủi và dịch vụ đưa đón.",
                        IsDeleted = ActiveEnums.Active
                    }
                };
                context.ServicePackages.AddRange(servicePackages);
                context.SaveChanges();
            }
// =============================================== PROMOTION ===============================================
            if (!context.Promotions.Any())
            {
                var promotions = new List<Promotion>
                {
                    new Promotion
                    {
                        PromotionCode = "WELCOME10",
                        PromotionName = "Giảm 10% cho khách hàng mới",
                        DiscountValue = 10m,
                        MinAmount = 500000m,
                        MaxDisCountAmount = 200000m,
                        UsageLimit = 100,
                        UsedCount = 0,
                        StartDate = new DateTime(2026, 1, 1),
                        EndDate = new DateTime(2026, 12, 31)
                    },
                    new Promotion
                    {
                        PromotionCode = "SUMMER15",
                        PromotionName = "Ưu đãi mùa hè",
                        DiscountValue = 15m,
                        MinAmount = 1000000m,
                        MaxDisCountAmount = 300000m,
                        UsageLimit = 50,
                        UsedCount = 0,
                        StartDate = new DateTime(2026, 5, 1),
                        EndDate = new DateTime(2026, 9, 30)
                    },
                    new Promotion
                    {
                        PromotionCode = "VIP20",
                        PromotionName = "Ưu đãi khách hàng VIP",
                        DiscountValue = 20m,
                        MinAmount = 2000000m,
                        MaxDisCountAmount = 500000m,
                        UsageLimit = 30,
                        UsedCount = 0,
                        StartDate = new DateTime(2026, 1, 1),
                        EndDate = new DateTime(2026, 12, 31)
                    }
                };
                context.Promotions.AddRange(promotions);
                context.SaveChanges();
            }
// =============================================== APPLY PROMOTION ===============================================
            if (!context.ApplyPromotions.Any())
            {
                var welcome10 = context.Promotions
                    .First(p => p.PromotionCode == "WELCOME10");
                var summer15 = context.Promotions
                    .First(p => p.PromotionCode == "SUMMER15");
                var vip20 = context.Promotions
                    .First(p => p.PromotionCode == "VIP20");
                var standard = context.RoomTypes
                    .First(rt => rt.RoomTypeName == "Standard");
                var deluxe = context.RoomTypes
                    .First(rt => rt.RoomTypeName == "Deluxe");
                var family = context.RoomTypes
                    .First(rt => rt.RoomTypeName == "Family");
                var suite = context.RoomTypes
                    .First(rt => rt.RoomTypeName == "Suite");

                var applyPromotions = new List<ApplyPromotion>
                {
                    // WELCOME10
                    new ApplyPromotion
                    {
                        PromotionId = welcome10.PromotionId,
                        RoomTypeId = standard.RoomTypeId
                    },
                    new ApplyPromotion
                    {
                        PromotionId = welcome10.PromotionId,
                        RoomTypeId = deluxe.RoomTypeId
                    },
                    new ApplyPromotion
                    {
                        PromotionId = welcome10.PromotionId,
                        RoomTypeId = family.RoomTypeId
                    },
                    // SUMMER15
                    new ApplyPromotion
                    {
                        PromotionId = summer15.PromotionId,
                        RoomTypeId = deluxe.RoomTypeId
                    },
                    new ApplyPromotion
                    {
                        PromotionId = summer15.PromotionId,
                        RoomTypeId = family.RoomTypeId
                    },
                    new ApplyPromotion
                    {
                        PromotionId = summer15.PromotionId,
                        RoomTypeId = suite.RoomTypeId
                    },
                    // VIP20
                    new ApplyPromotion
                    {
                        PromotionId = vip20.PromotionId,
                        RoomTypeId = family.RoomTypeId
                    },
                    new ApplyPromotion
                    {
                        PromotionId = vip20.PromotionId,
                        RoomTypeId = suite.RoomTypeId
                    }
                };
                context.ApplyPromotions.AddRange(applyPromotions);
                context.SaveChanges();
            }

// =============================================== CANCELLATION POLICY ===============================================
            if (!context.CancellationPolicies.Any())
            {
                var policies = new List<CancellationPolicy>
                {
                    new CancellationPolicy
                    {
                        PolicyName = "Hủy trước 7 ngày",
                        CancellationHours = 168,
                        RefundPercentage = 100m,
                        Description = "Hủy trước thời điểm nhận phòng từ 7 ngày trở lên được hoàn 100% tiền cọc.",
                        IsDeleted = ActiveEnums.Active
                    },
                    new CancellationPolicy
                    {
                        PolicyName = "Hủy trước 3 ngày",
                        CancellationHours = 72,
                        RefundPercentage = 70m,
                        Description = "Hủy trước thời điểm nhận phòng từ 3 đến dưới 7 ngày được hoàn 70% tiền cọc.",
                        IsDeleted = ActiveEnums.Active
                    },
                    new CancellationPolicy
                    {
                        PolicyName = "Hủy trước 24 giờ",
                        CancellationHours = 24,
                        RefundPercentage = 50m,
                        Description = "Hủy trước thời điểm nhận phòng từ 24 giờ đến dưới 3 ngày được hoàn 50% tiền cọc.",
                        IsDeleted = ActiveEnums.Active
                    },
                    new CancellationPolicy
                    {
                        PolicyName = "Hủy dưới 24 giờ",
                        CancellationHours = 0,
                        RefundPercentage = 0m,
                        Description = "Hủy trong vòng 24 giờ trước thời điểm nhận phòng không được hoàn tiền cọc.",
                        IsDeleted = ActiveEnums.Active
                    }
                };
                context.CancellationPolicies.AddRange(policies);
                context.SaveChanges();
            }

        }
    }
}
