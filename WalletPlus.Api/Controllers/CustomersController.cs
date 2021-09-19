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
using WalletPlus.Common.IRepository;
using WalletPlus.Common.Models;

namespace WalletPlus.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CustomersController> _logger;
        private readonly IMapper _mapper;

        public CustomersController(IUnitOfWork unitOfWork, ILogger<CustomersController> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        ////[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        ////[HttpCacheValidation(MustRevalidate = false)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomers([FromQuery] RequestParams requestParams)
        {
            var customers = await _unitOfWork.Customers.GetPagedList(requestParams, include: q => q.Include(x => x.Wallets).Include(x => x.Transactions));
            var results = _mapper.Map<IList<CustomerModel>>(customers);
            return Ok(results);
        }

        //[HttpGet("{id:Guid}", Name = "GetCustomer")]
        //[ResponseCache(CacheProfileName = "120SecondsDuration")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> GetCustomer(Guid id)
        //{
        //    var customer = await _unitOfWork.Customers.Get(q => q.Id == id, include: q => q.Include(x => x.Wallets));
        //    var result = _mapper.Map<CustomerModel>(customer);
        //    return Ok(result);
        //}

        [HttpGet("{customerId:Guid}/transactions", Name = "GetCustomerTransactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomerTransactions(Guid customerId)
        {
            var transactions = await _unitOfWork.Transactions.GetAll(c=> c.CustomerId == customerId || c.BeneficiaryCustomerId == customerId, include: q => q.Include(x => x.Customer).Include(x=> x.BeneficiaryCustomer));
            var results = _mapper.Map<IList<TransactionModel>>(transactions);
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

            var customer = _mapper.Map<Customer>(customerModel);

            customer.Id = Guid.NewGuid();
            customer.Active = true;

            await _unitOfWork.Customers.Insert(customer);

            //create wallet
            var transactionWallet = new Wallet
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                Type = WalletTypeEnum.Transactional,
                Amount = 0,
                CreatedDate = DateTime.Now 
            };
            await _unitOfWork.Wallets.Insert(transactionWallet);

            var pointWallet = new Wallet
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                Type = WalletTypeEnum.Point,
                Amount = 0,
                CreatedDate = DateTime.Now
            };
            await _unitOfWork.Wallets.Insert(pointWallet);

            await _unitOfWork.Save();

            return CreatedAtRoute("GetCustomer", new { id = customer.Id }, customer);

        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerModel customerModel)
        {
            if (!ModelState.IsValid || id == Guid.Empty)
            {
                _logger.LogError($"Invalid update attempt in {nameof(UpdateCustomer)}");
                return BadRequest(ModelState);
            }

            var customer = await _unitOfWork.Customers.Get(q => q.Id == id);
            if (customer == null)
            {
                _logger.LogError($"Invalid update attempt in {nameof(UpdateCustomer)}");
                return BadRequest("Submitted data is invalid");
            }

            _mapper.Map(customerModel, customer);
            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.Save();

            return NoContent();

        }
    }
}
