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
    public class TransactionsController : ControllerBase
    {
        private readonly ILogger<TransactionsController> _logger;
        private readonly ITransactionService _transactionService;

        public TransactionsController(ILogger<TransactionsController> logger,
            ITransactionService transactionService)
        {
            _logger = logger;
            _transactionService = transactionService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTransactions([FromQuery] RequestParams requestParams)
        {
            var results = await _transactionService.GetTransactions(requestParams);
            return Ok(results);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("{id:Guid}", Name = "GetTransaction")]
        [ResponseCache(CacheProfileName = "120SecondsDuration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTransaction(Guid id)
        {
            var result = await _transactionService.GetTransaction(id);
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

            try
            {
                var transaction = await _transactionService.FundWallet(fundWalletModel);
                return CreatedAtRoute("GetTransaction", new { id = transaction.Id }, transaction);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (WalletException ex)
            {
                return BadRequest(ex.Message);
            }
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

            try
            {
                var transaction = await _transactionService.MakeTransfer(makeTransferModel);
                return CreatedAtRoute("GetTransaction", new { id = transaction.Id }, transaction);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (WalletException ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
