using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Domain;
using Codemy.Identity.Domain.Entities;
using Codemy.Identity.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Action = Codemy.Identity.Domain.Entities.Action;
using RequestType = Codemy.Identity.Domain.Entities.RequestType;

namespace Codemy.Identity.Infrastructure.Persistence
{
    public class IdentityDbContext : DbContext
    {
        private readonly IEnumerable<Type> _entityTypes;
        private readonly PasswordHasher<string> _hasher = new();

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
            // Get all types from Domain assembly that inherit from BaseEntity
            _entityTypes = typeof(BaseEntity).Assembly
                .GetTypes()
                .Where(t => t is { IsAbstract: false, IsClass: true } && t.IsSubclassOf(typeof(BaseEntity)));
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Action> Actions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionGroup> PermissionGroups { get; set; }
        public DbSet<UserPermissionGroup> UserPermissionGroups { get; set; }

        public DbSet<RequestType> RequestTypes { get; set; }
        public DbSet<Request> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Register all entities
            foreach (var entityType in _entityTypes)
            {
                modelBuilder.Entity(entityType);
            }

            // Apply configurations (including seed data) 
            SeedAdmin(modelBuilder);
            SeedModerator(modelBuilder);
            SeedInstructor(modelBuilder);
            SeedAction(modelBuilder);
            SeedPermission(modelBuilder);
            SeedPermissionGroup(modelBuilder);
            SeedUserPermission(modelBuilder);
            SeedRequestType(modelBuilder);
        }

        private void SeedRequestType(ModelBuilder modelBuilder)
        {
            List<RequestType> requestTypes = new List<RequestType>
            {
                new RequestType { Id = Guid.Parse("55555555-5555-5555-5555-555555555551"), Type = RequestTypeEnum.UpgradeToInstructor, Description = "Request to upgrade user role to Instructor" },
                new RequestType { Id = Guid.Parse("55555555-5555-5555-5555-555555555552"), Type = RequestTypeEnum.PublicCourseRequest, Description = "Request to make a course public" },
                new RequestType { Id = Guid.Parse("55555555-5555-5555-5555-555555555553"), Type = RequestTypeEnum.HideCourseRequest, Description = "Request to hide a course" },
                new RequestType { Id = Guid.Parse("55555555-5555-5555-5555-555555555554"), Type = RequestTypeEnum.ReportCourseRequest, Description = "Request to report a course" },
                new RequestType { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Type = RequestTypeEnum.ReportReviewRequest, Description = "Request to report a review" },
            };
            modelBuilder.Entity<RequestType>().HasData(requestTypes);
        }

        private void SeedAction(ModelBuilder modelBuilder)
        {
            List<Action> actions = new List<Action>
            {
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444441"),  Name = "CATEGORY_CREATE", Code = "CATEGORY_CREATE", Description = "Create a new category" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444442"),  Name = "CATEGORY_READ", Code = "CATEGORY_READ", Description = "Read category" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444443"),  Name = "CATEGORY_UPDATE", Code = "CATEGORY_UPDATE", Description = "Update catogory information" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),  Name = "CATEGORY_DELETE", Code = "CATEGORY_DELETE", Description = "Delete category" },

                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444445"),  Name = "COURSE_CREATE", Code = "COURSE_CREATE", Description = "Create a new course" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444446"),  Name = "COURSE_READ", Code = "COURSE_READ", Description = "Read course" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444447"),  Name = "COURSE_UPDATE", Code = "COURSE_UPDATE", Description = "Update course information" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444448"),  Name = "COURSE_DELETE", Code = "COURSE_DELETE", Description = "Delete course" },

                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444449"),  Name = "LESSON_CREATE", Code = "LESSON_CREATE", Description = "Create a new lesson" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444450"),  Name = "LESSON_READ", Code = "LESSON_READ", Description = "Read lesson" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444451"),  Name = "LESSON_UPDATE", Code = "LESSON_UPDATE", Description = "Update lesson information" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444452"),  Name = "LESSON_DELETE", Code = "LESSON_DELETE", Description = "Delete lesson" },

                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444453"),  Name = "MODULE_CREATE", Code = "MODULE_CREATE", Description = "Create a new module" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444454"),  Name = "MODULE_READ", Code = "MODULE_READ", Description = "Read module" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444455"),  Name = "MODULE_UPDATE", Code = "MODULE_UPDATE", Description = "Update module information" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444456"),  Name = "MODULE_DELETE", Code = "MODULE_DELETE", Description = "Delete module" },

                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444457"),  Name = "QUIZ_CREATE", Code = "QUIZ_CREATE", Description = "Create a new quiz" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444458"),  Name = "QUIZ_READ", Code = "QUIZ_READ", Description = "Read quiz" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444459"),  Name = "QUIZ_UPDATE", Code = "QUIZ_UPDATE", Description = "Update quiz information" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444460"),  Name = "QUIZ_DELETE", Code = "QUIZ_DELETE", Description = "Delete quiz" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444461"),  Name = "QUIZ_SUBMIT", Code = "QUIZ_SUBMIT", Description = "Submit quiz" },

                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444462"),  Name = "ENROLLMENT_CREATE", Code = "ENROLLMENT_CREATE", Description = "Create a new enrollment" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444463"),  Name = "ENROLLMENT_READ", Code = "ENROLLMENT_READ", Description = "Read enrollment" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444464"),  Name = "ENROLLMENT_UPDATE", Code = "ENROLLMENT_UPDATE", Description = "Update enrollment information" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444465"),  Name = "ENROLLMENT_DELETE", Code = "ENROLLMENT_DELETE", Description = "Delete enrollment" },

                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444466"),  Name = "WISHLIST_CREATE", Code = "WISHLIST_CREATE", Description = "Create a new wishlist" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444467"),  Name = "WISHLIST_READ", Code = "WISHLIST_READ", Description = "Read wishlist" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444468"),  Name = "WISHLIST_UPDATE", Code = "WISHLIST_UPDATE", Description = "Update wishlist information" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444469"),  Name = "WISHLIST_DELETE", Code = "WISHLIST_DELETE", Description = "Delete wishlist" },

                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444470"),  Name = "USER_UPDATE", Code = "USER_UPDATE", Description = "Update user information" },

                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444471"),  Name = "PAYMENT_CREATE", Code = "PAYMENT_CREATE", Description = "Create a new payment" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444472"),  Name = "PAYMENT_READ", Code = "PAYMENT_READ", Description = "Read payment" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444473"),  Name = "PAYMENT_UPDATE", Code = "PAYMENT_UPDATE", Description = "Update payment information" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444474"),  Name = "PAYMENT_DELETE", Code = "PAYMENT_DELETE", Description = "Delete payment" },

                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444475"),  Name = "REVIEW_CREATE", Code = "REVIEW_CREATE", Description = "Create a new review" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444476"),  Name = "REVIEW_READ", Code = "REVIEW_READ", Description = "Read review" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444477"),  Name = "REVIEW_UPDATE", Code = "REVIEW_UPDATE", Description = "Update review information" },
                new Action { Id = Guid.Parse("44444444-4444-4444-4444-444444444478"),  Name = "REVIEW_DELETE", Code = "REVIEW_DELETE", Description = "Delete review" },
            };
            modelBuilder.Entity<Action>().HasData(actions);
        }

        private void SeedPermission(ModelBuilder modelBuilder)
        {
            List<Permission> permissions = new List<Permission>
            {
                new Permission { Id = Guid.Parse("44444444-4444-4444-4444-444444444494"), permissionName = "Category Permission" },

                new Permission { Id = Guid.Parse("44444444-4444-4444-4444-444444444495"), permissionName = "Course Permission" },

                new Permission { Id = Guid.Parse("44444444-4444-4444-4444-444444444496"), permissionName = "Lesson Permission" },

                new Permission { Id = Guid.Parse("44444444-4444-4444-4444-444444444497"), permissionName = "Module Permission" },

                new Permission { Id = Guid.Parse("44444444-4444-4444-4444-444444444498"), permissionName = "Quiz Permission" },

                new Permission { Id = Guid.Parse("44444444-4444-4444-4444-444444444499"), permissionName = "Enrollment Permission" },

                new Permission { Id = Guid.Parse("44444444-4444-4444-4444-444444444500"), permissionName = "Wishlist Permission" },

                new Permission { Id = Guid.Parse("44444444-4444-4444-4444-444444444501"), permissionName = "User Permission" },

                new Permission { Id = Guid.Parse("44444444-4444-4444-4444-444444444502"), permissionName = "Payment Permission" },

                new Permission { Id = Guid.Parse("44444444-4444-4444-4444-444444444503"), permissionName = "Review Permission" },

                new Permission { Id = Guid.Parse("44444444-4444-4444-4444-444444444542"), permissionName = "Student Permission"},

                new Permission { Id = Guid.Parse("44444444-4444-4444-4444-444444444562"), permissionName = "Other permission of Instructor"},


            };
            modelBuilder.Entity<Permission>().HasData(permissions);
        }

        private void SeedPermissionGroup(ModelBuilder modelBuilder)
        {
            List<PermissionGroup> permissionGroups = new List<PermissionGroup>
            {
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444504"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444494"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444441")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444505"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444494"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444442")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444506"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444494"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444443")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444507"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444494"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444444")},


                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444508"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444495"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444445")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444509"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444495"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444446")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444510"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444495"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444447")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444511"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444495"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444448")},


                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444512"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444496"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444449")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444513"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444496"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444450")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444514"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444496"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444451")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444515"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444496"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444452")},


                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444516"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444497"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444453")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444517"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444497"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444454")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444518"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444497"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444455")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444519"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444497"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444456")},


                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444520"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444498"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444457")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444521"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444498"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444458")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444522"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444498"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444459")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444523"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444498"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444460")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444524"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444498"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444461")},


                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444525"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444499"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444462")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444526"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444499"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444463")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444527"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444499"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444464")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444528"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444499"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444465")},


                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444529"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444500"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444466")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444530"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444500"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444467")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444531"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444500"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444468")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444532"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444500"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444469")},

                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444533"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444501"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444470")},

                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444534"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444502"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444471")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444535"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444502"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444472")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444536"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444502"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444473")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444537"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444502"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444474")},

                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444538"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444503"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444475")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444539"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444503"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444476")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444540"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444503"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444477")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444541"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444503"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444478")},

                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444542"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444442")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444543"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444446")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444544"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444450")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444545"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444454")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444546"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444458")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444547"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444461")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444548"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444462")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444549"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444463")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444550"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444464")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444551"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444466")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444552"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444467")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444553"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444468")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444554"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444469")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444555"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444470")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444556"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444471")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444557"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444472")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444558"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444473")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444559"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444475")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444560"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444476")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444561"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444542"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444477")},

                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444562"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444562"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444442")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444563"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444562"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444457")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444564"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444562"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444458")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444565"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444562"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444459")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444566"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444562"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444460")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444567"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444562"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444463")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444568"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444562"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444470")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444569"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444562"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444471")},
                new PermissionGroup { Id = Guid.Parse("44444444-4444-4444-4444-444444444570"), permissionId = Guid.Parse("44444444-4444-4444-4444-444444444562"), actionId = Guid.Parse("44444444-4444-4444-4444-444444444476")},




            };
            modelBuilder.Entity<PermissionGroup>().HasData(permissionGroups);
        }


        private void SeedUserPermission(ModelBuilder modelBuilder)
        {
            List<UserPermissionGroup> adminPermissions = new List<UserPermissionGroup>
            {
                new UserPermissionGroup{ Id = Guid.Parse("44444444-4444-4444-4444-444444444132"), RoleId = Role.Admin, PermissionId =  Guid.Parse("44444444-4444-4444-4444-444444444494")},

                new UserPermissionGroup{ Id = Guid.Parse("44444444-4444-4444-4444-444444444136"), RoleId = Role.Admin, PermissionId =  Guid.Parse("44444444-4444-4444-4444-444444444495")},

                new UserPermissionGroup{ Id = Guid.Parse("44444444-4444-4444-4444-444444444140"), RoleId = Role.Admin, PermissionId =  Guid.Parse("44444444-4444-4444-4444-444444444496")},

                new UserPermissionGroup{ Id = Guid.Parse("44444444-4444-4444-4444-444444444144"), RoleId = Role.Admin, PermissionId =  Guid.Parse("44444444-4444-4444-4444-444444444497")},

                new UserPermissionGroup{ Id = Guid.Parse("44444444-4444-4444-4444-444444444148"), RoleId = Role.Admin, PermissionId =  Guid.Parse("44444444-4444-4444-4444-444444444498")},

                new UserPermissionGroup{ Id = Guid.Parse("44444444-4444-4444-4444-444444444153"), RoleId = Role.Admin, PermissionId =  Guid.Parse("44444444-4444-4444-4444-444444444499")},

                new UserPermissionGroup{ Id = Guid.Parse("44444444-4444-4444-4444-444444444157"), RoleId = Role.Admin, PermissionId =  Guid.Parse("44444444-4444-4444-4444-444444444500")},

                new UserPermissionGroup{ Id = Guid.Parse("44444444-4444-4444-4444-444444444161"), RoleId = Role.Admin, PermissionId =  Guid.Parse("44444444-4444-4444-4444-444444444501")},

                new UserPermissionGroup{ Id = Guid.Parse("44444444-4444-4444-4444-444444444162"), RoleId = Role.Admin, PermissionId =  Guid.Parse("44444444-4444-4444-4444-444444444502")},

                new UserPermissionGroup{ Id = Guid.Parse("44444444-4444-4444-4444-444444444166"), RoleId = Role.Admin, PermissionId =  Guid.Parse("44444444-4444-4444-4444-444444444503")}

            };
            modelBuilder.Entity<UserPermissionGroup>().HasData(adminPermissions);


            List<UserPermissionGroup> insPermissions = new List<UserPermissionGroup>
            {

                new UserPermissionGroup{ Id = Guid.Parse("44444444-4444-4444-4444-444444444170"), RoleId = Role.Instructor, PermissionId =  Guid.Parse("44444444-4444-4444-4444-444444444495")},

                new UserPermissionGroup{ Id = Guid.Parse("44444444-4444-4444-4444-444444444174"), RoleId = Role.Instructor, PermissionId =  Guid.Parse("44444444-4444-4444-4444-444444444496")},

                new UserPermissionGroup{ Id = Guid.Parse("44444444-4444-4444-4444-444444444178"), RoleId = Role.Instructor, PermissionId =  Guid.Parse("44444444-4444-4444-4444-444444444497")},

                new UserPermissionGroup{ Id = Guid.Parse("44444444-4444-4444-4444-444444444182"), RoleId = Role.Instructor, PermissionId =  Guid.Parse("44444444-4444-4444-4444-444444444562")},


            };
            modelBuilder.Entity<UserPermissionGroup>().HasData(insPermissions);

            List<UserPermissionGroup> userPermissions = new List<UserPermissionGroup>
            {

                new UserPermissionGroup{ Id = Guid.Parse("44444444-4444-4444-4444-444444444192"), RoleId = Role.Student, PermissionId =  Guid.Parse("44444444-4444-4444-4444-444444444542")},

            };
            modelBuilder.Entity<UserPermissionGroup>().HasData(userPermissions);
        }

        


        

        private void SeedInstructor(ModelBuilder modelBuilder)
        {
            User instructor1 = new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444479"),
                
                name = "Nguyễn Hữu Khoa",
                email = "huukhoa04Ins@gmail.com",
                googleId = "",
                passwordHash = "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==",
                role = Role.Instructor,
                status = UserStatus.Active,
                emailVerified = true,
                totalLoginFailures = 0
            };
            User instructor2 = new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444480"),
                
                name = "Lê Xuân Hòa",
                email = "custina0987123Ins@gmail.com",
                googleId = "",
                passwordHash = "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==",
                role = Role.Instructor,
                status = UserStatus.Active,
                emailVerified = true,
                totalLoginFailures = 0
            };
            User instructor3 = new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444481"),
                
                name = "Huỳnh Thúy Minh Nguyệt",
                email = "minhnguyetdn2004Ins@gmail.com",
                googleId = "",
                passwordHash = "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==",
                role = Role.Instructor,
                status = UserStatus.Active,
                emailVerified = true,
                totalLoginFailures = 0
            };
            User instructor4 = new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444482"),
                
                name = "Đào Lê Hạnh Nguyên",
                email = "daolehanhnguyenIns@gmail.com",
                googleId = "",
                passwordHash = "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==",
                role = Role.Instructor,
                status = UserStatus.Active,
                emailVerified = true,
                totalLoginFailures = 0
            };
            User instructor5 = new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444483"),
                
                name = "Võ Văn Tuấn",
                email = "nauts010203Ins@gmail.com",
                googleId = "",
                passwordHash = "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==",
                role = Role.Instructor,
                status = UserStatus.Active,
                emailVerified = true,
                totalLoginFailures = 0
            };
            var ins = new List<User> { instructor1, instructor2, instructor3, instructor4, instructor5 };
            modelBuilder.Entity<User>().HasData(ins);
        }

        private void SeedModerator(ModelBuilder modelBuilder)
        {
            User mod1 = new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444484"),
                
                name = "Nguyễn Hữu Khoa",
                email = "huukhoa04Mod@gmail.com",
                googleId = "",
                passwordHash = "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==",
                role = Role.Moderator,
                status = UserStatus.Active,
                emailVerified = true,
                totalLoginFailures = 0
            };
            User mod2 = new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444485"),
                
                name = "Lê Xuân Hòa",
                email = "custina0987123Mod@gmail.com",
                googleId = "",
                passwordHash = "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==",
                role = Role.Moderator,
                status = UserStatus.Active,
                emailVerified = true,
                totalLoginFailures = 0
            };
            User mod3 = new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444486"),
                
                name = "Huỳnh Thúy Minh Nguyệt",
                email = "minhnguyetdn2004Mod@gmail.com",
                googleId = "",
                passwordHash = "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==",
                role = Role.Moderator,
                status = UserStatus.Active,
                emailVerified = true,
                totalLoginFailures = 0
            };
            User mod4 = new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444487"),
                
                name = "Đào Lê Hạnh Nguyên",
                email = "daolehanhnguyenMod@gmail.com",
                googleId = "",
                passwordHash = "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==",
                role = Role.Moderator,
                status = UserStatus.Active,
                emailVerified = true,
                totalLoginFailures = 0
            };
            User mod5 = new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444488"),
                
                name = "Võ Văn Tuấn",
                email = "nauts010203Mod@gmail.com",
                googleId = "",
                passwordHash = "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==",
                role = Role.Moderator,
                status = UserStatus.Active,
                emailVerified = true,
                totalLoginFailures = 0
            };
            var mods = new List<User> { mod1, mod2, mod3, mod4, mod5 };
            modelBuilder.Entity<User>().HasData(mods);
        }

        private void SeedAdmin(ModelBuilder modelBuilder)
        {
            User admin1 = new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444489"),
                
                name = "Nguyễn Hữu Khoa",
                email = "huukhoa04@gmail.com",
                googleId = "",
                passwordHash = "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==",
                role = Role.Admin,
                status = UserStatus.Active,
                emailVerified = true,
                totalLoginFailures = 0
            };
            User admin2 = new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444490"),
                
                name = "Lê Xuân Hòa",
                email = "custina0987123@gmail.com",
                googleId = "",
                passwordHash = "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==",
                role = Role.Admin,
                status = UserStatus.Active,
                emailVerified = true,
                totalLoginFailures = 0
            };
            User admin3 = new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444491"),
                
                name = "Huỳnh Thúy Minh Nguyệt",
                email = "minhnguyetdn2004@gmail.com",
                googleId = "",
                passwordHash = "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==",
                role = Role.Admin,
                status = UserStatus.Active,
                emailVerified = true,
                totalLoginFailures = 0
            };
            User admin4 = new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444492"),
                
                name = "Đào Lê Hạnh Nguyên",
                email = "daolehanhnguyen@gmail.com",
                googleId = "",
                passwordHash = "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==",
                role = Role.Admin,
                status = UserStatus.Active,
                emailVerified = true,
                totalLoginFailures = 0
            };
            User admin5 = new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444493"),
                name = "Võ Văn Tuấn",
                email = "nauts010203@gmail.com",
                googleId = "",
                passwordHash = "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==",
                role = Role.Admin,
                status = UserStatus.Active,
                emailVerified = true,
                totalLoginFailures = 0
            };
            var admins = new List<User> { admin1, admin2, admin3, admin4, admin5 };
            modelBuilder.Entity<User>().HasData(admins);
        }

        public DbSet<T> GetDbSet<T>() where T : class, IAuditableEntity
        {
            return Set<T>();
        }

        // Override SaveChanges to handle audit properties automatically
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedAt = null;
                        // TODO: Set CreatedBy from current user context if available
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        // TODO: Set UpdatedBy from current user context if available
                        break;
                }
            }

            // Handle BaseEntity specifically for additional properties like Id generation and soft delete
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        // Generate new Guid if not already set
                        if (entry.Entity.Id == Guid.Empty)
                        {
                            entry.Entity.Id = Guid.Parse("44444444-4444-4444-4444-444444444440");
                        }
                        break;
                    case EntityState.Deleted:
                        // Implement soft delete
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        // TODO: Set DeletedBy from current user context if available
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}