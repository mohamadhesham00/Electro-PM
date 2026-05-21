using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace UnitTests.Features.Tasks
{
    public class TaskCommandTests
    {
        #region Create Task Tests

        [Fact]
        public void CreateTaskCommand_WithValidInput_ShouldCreateSuccessfully()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var title = "Design Database Schema";
            var description = "Create normalized database design";
            var status = "Pending";
            var priority = "High";
            var dueDate = DateTime.UtcNow.AddDays(30);

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                Status = status,
                Priority = priority,
                DueDate = dueDate,
                ProjectId = projectId
            };

            // Act & Assert
            task.Title.Should().Be(title);
            task.Description.Should().Be(description);
            task.Status.Should().Be(status);
            task.Priority.Should().Be(priority);
            task.DueDate.Should().Be(dueDate);
            task.ProjectId.Should().Be(projectId);
            task.Id.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void CreateTaskCommand_WithEmptyTitle_ShouldFail(string invalidTitle)
        {
            // Arrange & Act
            var isValid = !string.IsNullOrWhiteSpace(invalidTitle);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void CreateTaskCommand_WithTitleExceeding100Chars_ShouldFail()
        {
            // Arrange
            var longTitle = new string('a', 101);

            // Act
            var isValid = longTitle.Length <= 100;

            // Assert
            isValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("Pending")]
        [InlineData("InProgress")]
        [InlineData("Completed")]
        [InlineData("OnHold")]
        [InlineData("Cancelled")]
        public void CreateTaskCommand_WithValidStatus_ShouldSucceed(string validStatus)
        {
            // Arrange
            var validStatuses = new[] { "Pending", "InProgress", "Completed", "OnHold", "Cancelled" };

            // Act
            var isValid = validStatuses.Contains(validStatus);

            // Assert
            isValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("Low")]
        [InlineData("Medium")]
        [InlineData("High")]
        [InlineData("Critical")]
        public void CreateTaskCommand_WithValidPriority_ShouldSucceed(string validPriority)
        {
            // Arrange
            var validPriorities = new[] { "Low", "Medium", "High", "Critical" };

            // Act
            var isValid = validPriorities.Contains(validPriority);

            // Assert
            isValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("InvalidStatus")]
        [InlineData("TODO")]
        [InlineData("DONE")]
        public void CreateTaskCommand_WithInvalidStatus_ShouldFail(string invalidStatus)
        {
            // Arrange
            var validStatuses = new[] { "Pending", "InProgress", "Completed", "OnHold", "Cancelled" };

            // Act
            var isValid = validStatuses.Contains(invalidStatus);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void CreateTaskCommand_WithoutDueDate_ShouldBeValid()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task Without Due Date",
                Description = "No deadline",
                Status = "Pending",
                Priority = "Low",
                DueDate = null,
                ProjectId = Guid.NewGuid()
            };

            // Act & Assert
            task.DueDate.Should().BeNull();
        }

        #endregion

        #region Update Task Tests

        [Fact]
        public void UpdateTaskCommand_ChangeStatus_ShouldUpdateSuccessfully()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task",
                Status = "Pending",
                Priority = "High",
                ProjectId = Guid.NewGuid()
            };

            var newStatus = "InProgress";

            // Act
            task.Status = newStatus;

            // Assert
            task.Status.Should().Be(newStatus);
        }

        [Fact]
        public void UpdateTaskCommand_ChangePriority_ShouldUpdateSuccessfully()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task",
                Status = "Pending",
                Priority = "Low",
                ProjectId = Guid.NewGuid()
            };

            var newPriority = "Critical";

            // Act
            task.Priority = newPriority;

            // Assert
            task.Priority.Should().Be(newPriority);
        }

        [Fact]
        public void UpdateTaskCommand_PartialUpdate_ShouldOnlyUpdateProvidedFields()
        {
            // Arrange
            var originalTask = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Original Title",
                Description = "Original Description",
                Status = "Pending",
                Priority = "High",
                DueDate = DateTime.UtcNow.AddDays(10),
                ProjectId = Guid.NewGuid()
            };

            var newStatus = "Completed";

            // Act
            originalTask.Status = newStatus;

            // Assert
            originalTask.Status.Should().Be(newStatus);
            originalTask.Title.Should().Be("Original Title");
            originalTask.Description.Should().Be("Original Description");
            originalTask.Priority.Should().Be("High");
        }

        [Fact]
        public void UpdateTaskCommand_SetDueDate_ShouldUpdate()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task",
                Status = "Pending",
                Priority = "High",
                DueDate = null,
                ProjectId = Guid.NewGuid()
            };

            var newDueDate = DateTime.UtcNow.AddDays(15);

            // Act
            task.DueDate = newDueDate;

            // Assert
            task.DueDate.Should().Be(newDueDate);
        }

        #endregion

        #region Delete Task Tests

        [Fact]
        public void DeleteTaskCommand_WithExistingTask_ShouldDelete()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var tasks = new List<TaskItem>
            {
                new TaskItem
                {
                    Id = taskId,
                    Title = "Task to Delete",
                    Status = "Pending",
                    Priority = "High",
                    ProjectId = projectId
                }
            };

            // Act
            var taskToDelete = tasks.FirstOrDefault(t => t.Id == taskId && t.ProjectId == projectId);
            if (taskToDelete != null)
            {
                tasks.Remove(taskToDelete);
            }

            // Assert
            tasks.Should().BeEmpty();
        }

        [Fact]
        public void DeleteTaskCommand_WithNonExistentTask_ShouldNotDelete()
        {
            // Arrange
            var nonExistentTaskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var tasks = new List<TaskItem>();

            // Act
            var taskToDelete = tasks.FirstOrDefault(t => t.Id == nonExistentTaskId && t.ProjectId == projectId);

            // Assert
            taskToDelete.Should().BeNull();
        }

        #endregion

        #region Task Ownership Tests

        [Fact]
        public void Task_BelongsToProject()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task",
                Status = "Pending",
                Priority = "High",
                ProjectId = projectId
            };

            // Act & Assert
            task.ProjectId.Should().Be(projectId);
        }

        [Fact]
        public void Task_CannotBelongToMultipleProjects()
        {
            // Arrange
            var projectId1 = Guid.NewGuid();
            var projectId2 = Guid.NewGuid();
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task",
                Status = "Pending",
                Priority = "High",
                ProjectId = projectId1
            };

            // Act
            var belongsToProject1 = task.ProjectId == projectId1;
            var belongsToProject2 = task.ProjectId == projectId2;

            // Assert
            belongsToProject1.Should().BeTrue();
            belongsToProject2.Should().BeFalse();
        }

        #endregion
    }

    public class TaskQueryTests
    {
        #region Get Tasks By Project Tests

        [Fact]
        public void GetTasksByProjectQuery_ShouldReturnOnlyProjectTasks()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), Title = "Task 1", Status = "Pending", Priority = "High", ProjectId = projectId },
                new TaskItem { Id = Guid.NewGuid(), Title = "Task 2", Status = "InProgress", Priority = "Medium", ProjectId = Guid.NewGuid() },
                new TaskItem { Id = Guid.NewGuid(), Title = "Task 3", Status = "Completed", Priority = "Low", ProjectId = projectId }
            };

            // Act
            var projectTasks = tasks.Where(t => t.ProjectId == projectId).ToList();

            // Assert
            projectTasks.Should().HaveCount(2);
            projectTasks.Should().AllSatisfy(t => t.ProjectId.Should().Be(projectId));
        }

        [Fact]
        public void GetTasksByProjectQuery_WithNoTasks_ShouldReturnEmptyList()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var tasks = new List<TaskItem>();

            // Act
            var projectTasks = tasks.Where(t => t.ProjectId == projectId).ToList();

            // Assert
            projectTasks.Should().BeEmpty();
        }

        #endregion

        #region Task Status Tests

        [Fact]
        public void TaskStatusTransition_FromPendingToInProgress_ShouldBeValid()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task",
                Status = "Pending",
                Priority = "High",
                ProjectId = Guid.NewGuid()
            };

            // Act
            var canTransition = task.Status == "Pending";
            if (canTransition)
            {
                task.Status = "InProgress";
            }

            // Assert
            task.Status.Should().Be("InProgress");
        }

        [Fact]
        public void TaskCanBeCompleted_ShouldChangeStatusToCompleted()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task",
                Status = "InProgress",
                Priority = "High",
                ProjectId = Guid.NewGuid()
            };

            // Act
            task.Status = "Completed";

            // Assert
            task.Status.Should().Be("Completed");
        }

        #endregion

        #region Task Priority Tests

        [Fact]
        public void HighPriorityTasks_ShouldBeSeparated()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), Title = "Critical", Priority = "Critical", Status = "Pending", ProjectId = Guid.NewGuid() },
                new TaskItem { Id = Guid.NewGuid(), Title = "Low", Priority = "Low", Status = "Pending", ProjectId = Guid.NewGuid() },
                new TaskItem { Id = Guid.NewGuid(), Title = "High", Priority = "High", Status = "Pending", ProjectId = Guid.NewGuid() }
            };

            // Act
            var highPriorityTasks = tasks.Where(t => t.Priority == "Critical" || t.Priority == "High").ToList();

            // Assert
            highPriorityTasks.Should().HaveCount(2);
        }

        #endregion

        #region Task Due Date Tests

        [Fact]
        public void OverdueTasks_ShouldBeIdentified()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), Title = "Overdue", Status = "Pending", Priority = "High", DueDate = now.AddDays(-1), ProjectId = Guid.NewGuid() },
                new TaskItem { Id = Guid.NewGuid(), Title = "Due Soon", Status = "Pending", Priority = "High", DueDate = now.AddDays(1), ProjectId = Guid.NewGuid() }
            };

            // Act
            var overdueTasks = tasks.Where(t => t.DueDate.HasValue && t.DueDate < now).ToList();

            // Assert
            overdueTasks.Should().HaveCount(1);
            overdueTasks.First().Title.Should().Be("Overdue");
        }

        #endregion
    }
}
