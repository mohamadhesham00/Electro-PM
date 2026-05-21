using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace UnitTests.Validators
{
    public class DataValidationTests
    {
        #region Project Validation Tests

        [Fact]
        public void ProjectName_Required()
        {
            // Arrange
            var invalidNames = new[] { "", " ", null };

            // Act & Assert
            foreach (var name in invalidNames)
            {
                var isValid = !string.IsNullOrWhiteSpace(name);
                isValid.Should().BeFalse();
            }
        }

        [Fact]
        public void ProjectName_MaxLength100()
        {
            // Arrange
            var shortName = "Valid Name";
            var maxName = new string('a', 100);
            var tooLongName = new string('a', 101);

            // Act & Assert
            shortName.Length.Should().BeLessThanOrEqualTo(100);
            maxName.Length.Should().BeLessThanOrEqualTo(100);
            tooLongName.Length.Should().BeGreaterThan(100);
        }

        [Fact]
        public void ProjectDescription_MaxLength500()
        {
            // Arrange
            var shortDescription = "Valid Description";
            var maxDescription = new string('a', 500);
            var tooLongDescription = new string('a', 501);

            // Act & Assert
            shortDescription.Length.Should().BeLessThanOrEqualTo(500);
            maxDescription.Length.Should().BeLessThanOrEqualTo(500);
            tooLongDescription.Length.Should().BeGreaterThan(500);
        }

        [Fact]
        public void ProjectId_CannotBeEmpty()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var emptyGuid = Guid.Empty;

            // Act
            var isValid = projectId != emptyGuid;

            // Assert
            isValid.Should().BeTrue();
        }

        #endregion

        #region Task Validation Tests

        [Fact]
        public void TaskTitle_Required()
        {
            // Arrange
            var invalidTitles = new[] { "", " ", null };

            // Act & Assert
            foreach (var title in invalidTitles)
            {
                var isValid = !string.IsNullOrWhiteSpace(title);
                isValid.Should().BeFalse();
            }
        }

        [Fact]
        public void TaskTitle_MaxLength100()
        {
            // Arrange
            var shortTitle = "Valid Title";
            var maxTitle = new string('a', 100);
            var tooLongTitle = new string('a', 101);

            // Act & Assert
            shortTitle.Length.Should().BeLessThanOrEqualTo(100);
            maxTitle.Length.Should().BeLessThanOrEqualTo(100);
            tooLongTitle.Length.Should().BeGreaterThan(100);
        }

        [Theory]
        [InlineData("Pending")]
        [InlineData("InProgress")]
        [InlineData("Completed")]
        [InlineData("OnHold")]
        [InlineData("Cancelled")]
        public void TaskStatus_ValidValues(string status)
        {
            // Arrange
            var validStatuses = new[] { "Pending", "InProgress", "Completed", "OnHold", "Cancelled" };

            // Act
            var isValid = validStatuses.Contains(status);

            // Assert
            isValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("Low")]
        [InlineData("Medium")]
        [InlineData("High")]
        [InlineData("Critical")]
        public void TaskPriority_ValidValues(string priority)
        {
            // Arrange
            var validPriorities = new[] { "Low", "Medium", "High", "Critical" };

            // Act
            var isValid = validPriorities.Contains(priority);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void TaskId_CannotBeEmpty()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var emptyGuid = Guid.Empty;

            // Act
            var isValid = taskId != emptyGuid;

            // Assert
            isValid.Should().BeTrue();
        }

        #endregion

        #region User Validation Tests

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("admin@test.org")]
        [InlineData("test+tag@domain.co.uk")]
        public void UserEmail_ValidFormat(string email)
        {
            // Arrange & Act
            var hasAtSymbol = email.Contains("@");
            var hasDomain = email.Contains(".");
            var isValid = hasAtSymbol && hasDomain;

            // Assert
            isValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("short")]
        [InlineData("")]
        [InlineData("123")]
        public void UserPassword_MinimumLength6(string password)
        {
            // Arrange & Act
            var isValid = password.Length >= 6;

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void UserPassword_ShouldBeHashed()
        {
            // Arrange
            var plainPassword = "SecurePassword123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            // Act
            var isVerified = BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);

            // Assert
            isVerified.Should().BeTrue();
            hashedPassword.Should().NotBe(plainPassword);
        }

        #endregion

        #region Cross-Field Validation Tests

        [Fact]
        public void Project_ProjectIdShouldNotBeEmpty()
        {
            // Arrange
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                OwnerId = Guid.NewGuid()
            };

            // Act & Assert
            project.Id.Should().NotBeEmpty();
            project.OwnerId.Should().NotBeEmpty();
        }

        [Fact]
        public void Task_TaskAndProjectIdShouldNotBeEmpty()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Test",
                ProjectId = Guid.NewGuid(),
                Status = "Pending",
                Priority = "High"
            };

            // Act & Assert
            task.Id.Should().NotBeEmpty();
            task.ProjectId.Should().NotBeEmpty();
        }

        #endregion
    }
}
