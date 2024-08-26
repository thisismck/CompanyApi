using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using CompanyApi.Controllers;
using CompanyApi.Data;
using CompanyApi.Models;

namespace CompanyApi.Tests
{
    public class CompaniesControllerTests
    {
        private readonly CompaniesController _controller;
        private readonly Mock<ApplicationDbContext> _mockContext;

        public CompaniesControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "CompanyDatabase")
                .Options;

            var context = new ApplicationDbContext(options);
            SeedDatabase(context);

            _mockContext = new Mock<ApplicationDbContext>(options);
            _controller = new CompaniesController(context);
        }

        private void SeedDatabase(ApplicationDbContext context)
        {
            context.Companies.Add(new Company {  Name = "Test Company", Isin = "US1234567890", Exchange = "NYSE", Ticker = "TST", Website = "http://www.testcompany.com" });
            context.SaveChanges();
        }

        [Fact]
        public async Task GetCompanies_ReturnsListOfCompanies()
        {
            // Act
            var result = await _controller.GetCompanies();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Company>>>(result);
            var companies = Assert.IsType<List<Company>>(actionResult.Value);
            Assert.Single(companies);
        }

        [Fact]
        public async Task GetCompanyById_ReturnsCompany()
        {
            // Act
            var result = await _controller.GetCompanyById(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Company>>(result);
            var company = Assert.IsType<Company>(actionResult.Value);
            Assert.Equal(1, company.Id);
        }

        [Fact]
        public async Task GetCompanyById_ReturnsNotFound_WhenCompanyDoesNotExist()
        {
            // Act
            var result = await _controller.GetCompanyById(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCompanyByIsin_ReturnsCompany()
        {
            // Act
            var result = await _controller.GetCompanyByIsin("US1234567890");

            // Assert
            var actionResult = Assert.IsType<ActionResult<Company>>(result);
            var company = Assert.IsType<Company>(actionResult.Value);
            Assert.Equal("US1234567890", company.Isin);
        }


        [Fact]
        public async Task CreateCompany_ReturnsBadRequest_WhenCompanyExists()
        {
            // Arrange
            var company = new Company { Name = "Test Company", Isin = "US1234567890", Exchange = "NYSE", Ticker = "TST", Website = "http://www.testcompany.com" };

            // Act
            var result = await _controller.CreateCompany(company);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }


        [Fact]
        public async Task Edit_ReturnsNotFound_WhenCompanyDoesNotExist()
        {
            // Arrange
            var company = new Company { Id = 999, Name = "Updated Company", Isin = "US1234567890", Exchange = "NYSE", Ticker = "TST", Website = "http://www.updatedcompany.com" };

            // Act
            var result = await _controller.Edit(999, company);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
