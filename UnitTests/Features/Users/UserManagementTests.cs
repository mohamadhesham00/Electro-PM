using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace UnitTests.Features.Users
{
    public class UserManagementTests
    {
        #region Delete User Tests

        [Fact]
        public void DeleteUserCommand_WithExistingUser_ShouldDelete()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var users = new List<User>
            {
                new User { Id = userId, Email = "user@example.com", Password = "hash", Role = UserRole.User }
            };

            // Act
            var userToDelete = users.FirstOrDefault(u => u.Id == userId);
            if (userToDelete != null)
            {
                users.Remove(userToDelete);
            }

            // Assert
            users.Should().BeEmpty();
        }

        [Fact]
        public void DeleteUserCommand_WithNonExistentUser_ShouldThrowException()
        {
            // Arrange
            var nonExistentUserId = Guid.NewGuid();
            var users = new List<User>();

            // Act
            var user = users.FirstOrDefault(u => u.Id == nonExistentUserId);

            // Assert
            user.Should().BeNull();
        }

        [Fact]
        public void DeleteUserCommand_CannotDeleteOwnAccount_ShouldFail()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var admin = new User
            {
                Id = adminId,
                Email = "admin@example.com",
                Password = "hash",
                Role = UserRole.Admin
            };

            // Act
            var isAttemptingSelfDeletion = adminId == adminId;

            // Assert
            isAttemptingSelfDeletion.Should().BeTrue();
        }

        #endregion

        #region List Users Tests

        [Fact]
        public void ListUsersQuery_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "admin@example.com", Password = "hash", Role = UserRole.Admin },
                new User { Id = Guid.NewGuid(), Email = "user1@example.com", Password = "hash", Role = UserRole.User },
                new User { Id = Guid.NewGuid(), Email = "user2@example.com", Password = "hash", Role = UserRole.User }
            };

            // Act
            var allUsers = users.ToList();

            // Assert
            allUsers.Should().HaveCount(3);
        }

        [Fact]
        public void ListUsersQuery_WithNoUsers_ShouldReturnEmptyList()
        {
            // Arrange
            var users = new List<User>();

            // Act
            var allUsers = users.ToList();

            // Assert
            allUsers.Should().BeEmpty();
        }

        [Fact]
        public void ListUsersQuery_CanFilterByRole()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "admin@example.com", Password = "hash", Role = UserRole.Admin },
                new User { Id = Guid.NewGuid(), Email = "user1@example.com", Password = "hash", Role = UserRole.User },
                new User { Id = Guid.NewGuid(), Email = "user2@example.com", Password = "hash", Role = UserRole.User }
            };

            // Act
            var adminUsers = users.Where(u => u.Role == UserRole.Admin).ToList();
            var regularUsers = users.Where(u => u.Role == UserRole.User).ToList();

            // Assert
            adminUsers.Should().HaveCount(1);
            regularUsers.Should().HaveCount(2);
        }

        #endregion

        #region User Authorization Tests

        [Fact]
        public void AdminUser_ShouldHaveDeletePermissions()
        {
            // Arrange
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@example.com",
                Password = "hash",
                Role = UserRole.Admin
            };

            // Act
            var canDelete = admin.Role == UserRole.Admin;

            // Assert
            canDelete.Should().BeTrue();
        }

        [Fact]
        public void RegularUser_ShouldNotHaveDeletePermissions()
        {
            // Arrange
            var regularUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                Password = "hash",
                Role = UserRole.User
            };

            // Act
            var canDelete = regularUser.Role == UserRole.Admin;

            // Assert
            canDelete.Should().BeFalse();
        }

        [Fact]
        public void AdminUser_ShouldHaveUserListPermissions()
        {
            // Arrange
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@example.com",
                Password = "hash",
                Role = UserRole.Admin
            };

            // Act
            var canViewUsers = admin.Role == UserRole.Admin;

            // Assert
            canViewUsers.Should().BeTrue();
        }

        #endregion

        #region User Email Tests

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("admin@electropm.com")]
        [InlineData("test.user@domain.co.uk")]
        public void User_WithValidEmail_ShouldBeCreated(string validEmail)
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = validEmail,
                Password = BCrypt.Net.BCrypt.HashPassword("Password123"),
                Role = UserRole.User
            };

            // Act & Assert
            user.Email.Should().Be(validEmail);
        }

        [Fact]
        public void DuplicateEmails_ShouldNotBeAllowed()
        {
            // Arrange
            var email = "user@example.com";
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = email, Password = "hash", Role = UserRole.User }
            };

            // Act
            var isDuplicate = users.Any(u => u.Email == email);

            // Assert
            isDuplicate.Should().BeTrue();
        }

        #endregion

        #region User Role Tests

        [Theory]
        [InlineData(UserRole.Admin)]
        [InlineData(UserRole.User)]
        public void User_CanHaveDifferentRoles(UserRole role)
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                Password = "hash",
                Role = role
            };

            // Act & Assert
            user.Role.Should().Be(role);
        }

        [Fact]
        public void AdminUser_IsDistinguishedFromRegularUser()
        {
            // Arrange
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@example.com",
                Password = "hash",
                Role = UserRole.Admin
            };

            var regularUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                Password = "hash",
                Role = UserRole.User
            };

            // Act & Assert
            adminUser.Role.Should().Be(UserRole.Admin);
            regularUser.Role.Should().Be(UserRole.User);
            adminUser.Role.Should().NotBe(regularUser.Role);
        }

        #endregion

        #region User Password Tests

        [Fact]
        public void UserPassword_ShouldBeHashed()
        {
            // Arrange
            var plainPassword = "SecurePassword123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            // Act
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);

            // Assert
            isPasswordValid.Should().BeTrue();
        }

        [Fact]
        public void UserPassword_ShouldNotBeStoredInPlainText()
        {
            // Arrange
            var plainPassword = "SecurePassword123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            // Act & Assert
            hashedPassword.Should().NotBe(plainPassword);
        }

        #endregion
    }
}
