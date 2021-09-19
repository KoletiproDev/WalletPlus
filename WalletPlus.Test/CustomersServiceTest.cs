using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletPlus.Api.Controllers;
using WalletPlus.Common;
using WalletPlus.Common.Entities;
using WalletPlus.Common.IService;
using WalletPlus.Common.Models;
using Xunit;

namespace WalletPlus.Test
{
    public class CustomersServiceTest
    {
        ICustomerService _customerService;
        private static IMapper _mapper;

        public CustomersServiceTest()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MapperInitilizer());
                });

                IMapper mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }

            _customerService = new CustomerServiceFake(_mapper);
        }

        [Fact]
        public async Task GetCustomers_WhenCalled_ReturnsResult()
        {
            // Act
            var requestParam = new RequestParams
            {
                PageNumber = 1,
                PageSize = 10
            };

            var result = await _customerService.GetCustomers(requestParam);

            // Assert
            Assert.IsType<List<CustomerModel>>(result);
        }

        [Fact]
        public async Task Add_ValidObjectPassed_ReturnsCreatedObject()
        {
            // Arrange
            var testItem = new CreateCustomerModel()
            {
                Name = "Test3 Test3",
            };

            // Act
            var createdObject = await _customerService.CreateCustomer(testItem);

            // Assert
            Assert.IsType<CustomerModel>(createdObject);
        }
    }
}
