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
    public class TransactionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionsController> _logger;
        private readonly IMapper _mapper;

        public TransactionsController(IUnitOfWork unitOfWork, ILogger<TransactionsController> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTransactions([FromQuery] RequestParams requestParams)
        {
            var transactions = await _unitOfWork.Transactions.GetPagedList(requestParams);
            var results = _mapper.Map<IList<TransactionModel>>(transactions);
            return Ok(results);
        }

        [HttpGet("{id:Guid}", Name = "GetTransaction")]
        [ResponseCache(CacheProfileName = "120SecondsDuration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTransaction(Guid id)
        {
            var transaction = await _unitOfWork.Transactions.Get(q => q.Id == id, include: q => q.Include(x => x.Customer));
            var result = _mapper.Map<TransactionModel>(transaction);
            return Ok(result);
        }

        [HttpPost("fundwallet")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FundWallet([FromBody] FundWalletModel fundWalletModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid transaction attempt in {nameof(FundWallet)}");
                return BadRequest(ModelState);
            }

            var transaction = _mapper.Map<Transaction>(fundWalletModel);
            await _unitOfWork.Transactions.Insert(transaction);

            //update wallet

            await _unitOfWork.Save();

            return CreatedAtRoute("GetTransaction", new { id = transaction.Id }, transaction);

        }

        [HttpPost("maketransfer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MakeTransfer([FromBody] MakeTransferModel makeTransferModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid transaction attempt in {nameof(MakeTransfer)}");
                return BadRequest(ModelState);
            }

            var transaction = _mapper.Map<Transaction>(makeTransferModel);
            await _unitOfWork.Transactions.Insert(transaction);

            //update wallet

            await _unitOfWork.Save();

            return CreatedAtRoute("GetTransaction", new { id = transaction.Id }, transaction);

        }
    }
}
