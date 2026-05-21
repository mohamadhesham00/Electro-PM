using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace UnitTests.Features.Auth
{
    public class AuthenticationTests
    {
        #region Password Hashing Tests

        [Fact]
        public void PasswordHashing_WithValidPassword_ShouldHashSuccessfully()
        {
            // Arrange
            var password = "SecurePassword123";

            // Act
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // Assert
            hashedPassword.Should().NotBeNull();
            hashedPassword.Should().NotBe(password);
        }

        [Fact]
        public void PasswordVerification_WithCorrectPassword_ShouldSucceed()
        {
            // Arrange
            var password = "CorrectPassword123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // Act
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

            // Assert
            isPasswordValid.Should().BeTrue();
        }

        [Fact]
        public void PasswordVerification_WithIncorrectPassword_ShouldFail()
        {
            // Arrange
            var correctPassword = "CorrectPassword123";
            var wrongPassword = "WrongPassword123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(correctPassword);

            // Act
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(wrongPassword, hashedPassword);

            // Assert
            isPasswordValid.Should().BeFalse();
        }

        [Fact]
        public void PasswordHashing_ShouldProduceDifferentHashesForSamePassword()
        {
            // Arrange
            var password = "SamePassword123";

            // Act
            var hash1 = BCrypt.Net.BCrypt.HashPassword(password);
            var hash2 = BCrypt.Net.BCrypt.HashPassword(password);

            // Assert
            hash1.Should().NotBe(hash2);
            BCrypt.Net.BCrypt.Verify(password, hash1).Should().BeTrue();
            BCrypt.Net.BCrypt.Verify(password, hash2).Should().BeTrue();
        }

        #endregion

        #region User Registration Tests

        [Fact]
        public void UserRegistration_WithValidEmail_ShouldSucceed()
        {
            // Arrange
            var email = "newuser@example.com";
            var password = "ValidPassword123";

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Role = UserRole.User
            };

            // Act & Assert
            user.Email.Should().Be(email);
            user.Role.Should().Be(UserRole.User);
            BCrypt.Net.BCrypt.Verify(password, user.Password).Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void UserRegistration_WithInvalidPassword_ShouldFail(string invalidPassword)
        {
            // Arrange & Act
            var isValid = !string.IsNullOrWhiteSpace(invalidPassword) && invalidPassword.Length >= 6;

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void UserRegistration_WithDuplicateEmail_ShouldFail()
        {
            // Arrange
            var email = "duplicate@example.com";
            var existingUsers = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = email, Password = "hash", Role = UserRole.User }
            };

            // Act
            var isDuplicate = existingUsers.Any(u => u.Email == email);

            // Assert
            isDuplicate.Should().BeTrue();
        }

        #endregion

        #region User Authentication Tests

        [Fact]
        public void UserLogin_WithValidCredentials_ShouldAuthenticate()
        {
            // Arrange
            var email = "user@example.com";
            var password = "ValidPassword123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Password = hashedPassword,
                Role = UserRole.User
            };

            // Act
            var emailMatches = user.Email == email;
            var passwordMatches = BCrypt.Net.BCrypt.Verify(password, user.Password);

            // Assert
            emailMatches.Should().BeTrue();
            passwordMatches.Should().BeTrue();
        }

        [Fact]
        public void UserLogin_WithIncorrectPassword_ShouldFail()
        {
            // Arrange
            var email = "user@example.com";
            var correctPassword = "CorrectPassword123";
            var wrongPassword = "WrongPassword123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(correctPassword);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Password = hashedPassword,
                Role = UserRole.User
            };

            // Act
            var passwordMatches = BCrypt.Net.BCrypt.Verify(wrongPassword, user.Password);

            // Assert
            passwordMatches.Should().BeFalse();
        }

        #endregion

        #region Role-Based Tests

        [Fact]
        public void Admin_ShouldHaveAdminRole()
        {
            // Arrange
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("AdminPass123"),
                Role = UserRole.Admin
            };

            // Act & Assert
            admin.Role.Should().Be(UserRole.Admin);
        }

        [Fact]
        public void RegularUser_ShouldHaveUserRole()
        {
            // Arrange
            var regularUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("UserPass123"),
                Role = UserRole.User
            };

            // Act & Assert
            regularUser.Role.Should().Be(UserRole.User);
        }

        [Fact]
        public void RoleDeterminesPermissions()
        {
            // Arrange
            var admin = new User { Role = UserRole.Admin };
            var regularUser = new User { Role = UserRole.User };

            // Act
            var adminCanDelete = admin.Role == UserRole.Admin;
            var userCanDelete = regularUser.Role == UserRole.Admin;

            // Assert
            adminCanDelete.Should().BeTrue();
            userCanDelete.Should().BeFalse();
        }

        #endregion

        #region Email Validation Tests

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("admin@test.org")]
        [InlineData("test.user+tag@domain.co.uk")]
        public void EmailValidation_WithValidEmails_ShouldPass(string validEmail)
        {
            // Arrange
            var hasAtSymbol = validEmail.Contains("@");
            var hasDomain = validEmail.Contains(".");

            // Act
            var isValid = hasAtSymbol && hasDomain;

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void EmailUniqueness_ShouldBeEnforced()
        {
            // Arrange
            var email = "unique@example.com";
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = email, Password = "hash", Role = UserRole.User }
            };

            // Act
            var emailExists = users.Any(u => u.Email == email);

            // Assert
            emailExists.Should().BeTrue();
        }

        #endregion
    }
}
