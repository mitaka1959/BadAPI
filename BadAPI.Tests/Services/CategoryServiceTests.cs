using BadApi.Services;
using BadAPI.Data.Entities;
using BadAPI.Data.Repositories;
using BadAPI.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace BadAPI.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<IRepository<Category>> _repo = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();

        private CategoryService CreateSut() => new(_repo.Object, _unitOfWork.Object);

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task AddCategoryAsync_ReturnsInvalid_WhenNameMissing(string? name)
        {
            var result = await CreateSut().AddCategoryAsync(name!, "some description");

            Assert.Equal(ResultStatus.Invalid, result.Status);
            _repo.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Never);
            _unitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task AddCategoryAsync_ReturnsConflict_WhenNameAlreadyExists()
        {
            _repo.Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(new Category { Name = "Tools" });

            var result = await CreateSut().AddCategoryAsync("Tools", "dupe");

            Assert.Equal(ResultStatus.Conflict, result.Status);
            _repo.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task AddCategoryAsync_Succeeds_WhenNameIsNewAndValid()
        {
            _repo.Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync((Category?)null);

            var result = await CreateSut().AddCategoryAsync("Tools", "Hand tools");

            Assert.True(result.IsSuccess);
            Assert.Equal("Tools", result.Value!.Name);
            _repo.Verify(r => r.AddAsync(It.Is<Category>(c =>
                c.Name == "Tools" && c.Description == "Hand tools")), Times.Once);
            _unitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsAllFromRepository()
        {
            _repo.Setup(r => r.GetAllAsync())
                 .ReturnsAsync(new List<Category> { new(), new(), new() });

            var result = await CreateSut().GetCategoriesAsync();

            Assert.Equal(3, result.Count());
        }
    }
}
