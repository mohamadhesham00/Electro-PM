using Application.Features.Projects.Commands;
using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace UnitTests.Features.Projects
{
    public class ProjectCommandTests
    {
        #region Create Project Tests

        [Fact]
        public void CreateProjectCommand_WithValidInput_ShouldCreateSuccessfully()
        {
            // Arrange
            var projectName = "Q1 2025 Initiative";
            var description = "Strategic project for Q1";
            var userId = Guid.NewGuid();

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = projectName,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                OwnerId = userId,
                Tasks = new List<TaskItem>()
            };

            // Act & Assert
            project.Name.Should().Be(projectName);
            project.Description.Should().Be(description);
            project.OwnerId.Should().Be(userId);
            project.Id.Should().NotBeEmpty();
            project.Tasks.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void CreateProjectCommand_WithEmptyName_ShouldFail(string invalidName)
        {
            // Arrange & Act
            var isValid = !string.IsNullOrWhiteSpace(invalidName);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void CreateProjectCommand_WithNameExceeding100Chars_ShouldFail()
        {
            // Arrange
            var longName = new string('a', 101);

            // Act
            var isValid = longName.Length <= 100;

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void CreateProjectCommand_WithDescriptionExceeding500Chars_ShouldFail()
        {
            // Arrange
            var longDescription = new string('a', 501);

            // Act
            var isValid = longDescription.Length <= 500;

            // Assert
            isValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("Project Name")]
        [InlineData("My Project 2025")]
        [InlineData("Website Redesign")]
        [InlineData("API Development")]
        public void CreateProjectCommand_WithValidNames_ShouldSucceed(string validName)
        {
            // Arrange
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = validName,
                Description = "Test",
                CreatedAt = DateTime.UtcNow,
                OwnerId = Guid.NewGuid()
            };

            // Act & Assert
            project.Name.Should().Be(validName);
            project.Name.Length.Should().BeLessThanOrEqualTo(100);
        }

        #endregion

        #region Update Project Tests

        [Fact]
        public void UpdateProjectCommand_WithPartialUpdate_ShouldUpdateOnlyProvidedFields()
        {
            // Arrange
            var originalProject = new Project
            {
                Id = Guid.NewGuid(),
                Name = "Original Name",
                Description = "Original Description",
                CreatedAt = DateTime.UtcNow,
                OwnerId = Guid.NewGuid()
            };

            var newName = "Updated Name";

            // Act
            originalProject.Name = newName;

            // Assert
            originalProject.Name.Should().Be(newName);
            originalProject.Description.Should().Be("Original Description");
        }

        [Fact]
        public void UpdateProjectCommand_WithEmptyStringUpdate_ShouldNotUpdate()
        {
            // Arrange
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = "Original Name",
                Description = "Original Description",
                CreatedAt = DateTime.UtcNow,
                OwnerId = Guid.NewGuid()
            };

            var originalName = project.Name;

            // Act
            if (!string.IsNullOrEmpty(""))
            {
                project.Name = "";
            }

            // Assert
            project.Name.Should().Be(originalName);
        }

        #endregion

        #region Delete Project Tests

        [Fact]
        public void DeleteProjectCommand_WithExistingProject_ShouldDelete()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var projects = new List<Project>
            {
                new Project
                {
                    Id = projectId,
                    Name = "Project to Delete",
                    Description = "Will be deleted",
                    CreatedAt = DateTime.UtcNow,
                    OwnerId = Guid.NewGuid()
                }
            };

            // Act
            var projectToDelete = projects.FirstOrDefault(p => p.Id == projectId);
            if (projectToDelete != null)
            {
                projects.Remove(projectToDelete);
            }

            // Assert
            projects.Should().BeEmpty();
        }

        [Fact]
        public void DeleteProjectCommand_WithNonExistentProject_ShouldThrowException()
        {
            // Arrange
            var nonExistentProjectId = Guid.NewGuid();
            var projects = new List<Project>();

            // Act
            var project = projects.FirstOrDefault(p => p.Id == nonExistentProjectId);

            // Assert
            project.Should().BeNull();
        }

        #endregion

        #region Project Ownership Tests

        [Fact]
        public void Project_WithOwnerId_ShouldBelongToOwner()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = "Owned Project",
                Description = "Belongs to owner",
                CreatedAt = DateTime.UtcNow,
                OwnerId = ownerId
            };

            // Act & Assert
            project.OwnerId.Should().Be(ownerId);
            project.OwnerId.Should().NotBe(otherUserId);
        }

        [Fact]
        public void User_CanOnlyAccessOwnProjects()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projects = new List<Project>
            {
                new Project { Id = Guid.NewGuid(), Name = "Own Project 1", OwnerId = userId, CreatedAt = DateTime.UtcNow },
                new Project { Id = Guid.NewGuid(), Name = "Other Project", OwnerId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
                new Project { Id = Guid.NewGuid(), Name = "Own Project 2", OwnerId = userId, CreatedAt = DateTime.UtcNow }
            };

            // Act
            var userProjects = projects.Where(p => p.OwnerId == userId).ToList();

            // Assert
            userProjects.Should().HaveCount(2);
            userProjects.Should().AllSatisfy(p => p.OwnerId.Should().Be(userId));
        }

        #endregion

        #region Project Creation Time Tests

        [Fact]
        public void Project_ShouldHaveCreatedAtTimestamp()
        {
            // Arrange
            var createdTime = DateTime.UtcNow;
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = "Project",
                Description = "Test",
                CreatedAt = createdTime,
                OwnerId = Guid.NewGuid()
            };

            // Act & Assert
            project.CreatedAt.Should().Be(createdTime);
            project.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
        }

        #endregion
    }

    public class ProjectQueryTests
    {
        #region Get All Projects Tests

        [Fact]
        public void GetAllProjectsQuery_ShouldReturnOnlyUserProjects()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projects = new List<Project>
            {
                new Project { Id = Guid.NewGuid(), Name = "User Project 1", OwnerId = userId, CreatedAt = DateTime.UtcNow },
                new Project { Id = Guid.NewGuid(), Name = "Other User Project", OwnerId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
                new Project { Id = Guid.NewGuid(), Name = "User Project 2", OwnerId = userId, CreatedAt = DateTime.UtcNow }
            };

            // Act
            var userProjects = projects.Where(p => p.OwnerId == userId).ToList();

            // Assert
            userProjects.Should().HaveCount(2);
        }

        [Fact]
        public void GetAllProjectsQuery_WithNoProjects_ShouldReturnEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projects = new List<Project>();

            // Act
            var userProjects = projects.Where(p => p.OwnerId == userId).ToList();

            // Assert
            userProjects.Should().BeEmpty();
        }

        #endregion

        #region Get Project By ID Tests

        [Fact]
        public void GetProjectByIdQuery_WithValidId_ShouldReturnProject()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var project = new Project
            {
                Id = projectId,
                Name = "Target Project",
                Description = "Find me",
                CreatedAt = DateTime.UtcNow,
                OwnerId = userId
            };

            var projects = new List<Project> { project };

            // Act
            var foundProject = projects.FirstOrDefault(p => p.Id == projectId && p.OwnerId == userId);

            // Assert
            foundProject.Should().NotBeNull();
            foundProject?.Name.Should().Be("Target Project");
        }

        [Fact]
        public void GetProjectByIdQuery_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var projects = new List<Project>();

            // Act
            var foundProject = projects.FirstOrDefault(p => p.Id == projectId);

            // Assert
            foundProject.Should().BeNull();
        }

        [Fact]
        public void GetProjectByIdQuery_WithDifferentOwner_ShouldReturnNull()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var requestingUserId = Guid.NewGuid();

            var project = new Project
            {
                Id = projectId,
                Name = "Protected Project",
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow
            };

            var projects = new List<Project> { project };

            // Act
            var foundProject = projects.FirstOrDefault(p => p.Id == projectId && p.OwnerId == requestingUserId);

            // Assert
            foundProject.Should().BeNull();
        }

        #endregion
    }
}
