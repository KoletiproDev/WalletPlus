using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletPlus.Common.Entities;
using WalletPlus.Common.Exceptions;
using WalletPlus.Common.IRepository;
using WalletPlus.Common.IService;
using WalletPlus.Common.Models;

namespace WalletPlus.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ILogger<CustomersController> _logger;
        private readonly ICustomerService _customerService;

        public CustomersController(ILogger<CustomersController> logger,
            ICustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        [HttpGet]
        ////[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        ////[HttpCacheValidation(MustRevalidate = false)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomers([FromQuery] RequestParams requestParams)
        {
            var results = await _customerService.GetCustomers(requestParams);
            return Ok(results);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("{id:Guid}", Name = "GetCustomer")]
        [ResponseCache(CacheProfileName = "120SecondsDuration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomer(Guid id)
        {
            var result = await _customerService.GetCustomer(id);
            return Ok(result);
        }

        [HttpGet("{customerId:Guid}/transactions", Name = "GetCustomerTransactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomerTransactions(Guid customerId)
        {
            var results = await _customerService.GetCustomerTransactions(customerId);
            return Ok(results);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerModel customerModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateCustomer)}");
                return BadRequest(ModelState);
            }

            var customer = await _customerService.CreateCustomer(customerModel);

            return CreatedAtRoute("GetCustomer", new { id = customer.Id }, customer);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerModel updateCustomerModel)
        {
            if (!ModelState.IsValid || id == Guid.Empty)
            {
                _logger.LogError($"Invalid update attempt in {nameof(UpdateCustomer)}");
                return BadRequest(ModelState);
            }

            try
            {
                await _customerService.UpdateCustomer(id, updateCustomerModel);
            }
            catch(WalletException ex)
            {
                _logger.LogError($"Invalid update attempt in {nameof(UpdateCustomer)}");
                return BadRequest(ex.Message);
            }

            return NoContent();

        }
    }
}
