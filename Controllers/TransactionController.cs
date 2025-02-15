using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionRepository _transactionRepository;
        private readonly IValidator<TransactionModel> _transactionValidator;

        public TransactionController(TransactionRepository transactionRepository, IValidator<TransactionModel> transactionValidator)
        {
            _transactionRepository = transactionRepository;
            _transactionValidator = transactionValidator;
        }

        #region GetAllTransactions

        [HttpGet("/api/Transactions/{UserID}")]
        public IActionResult GetAllTransactions(int UserID)
        {
            try
            {
                // Fetch all transactions using the repository
                var transactions = _transactionRepository.GetAllTransactions(UserID);

                // Return the serialized list of transactions as JSON
                return Ok(JsonConvert.SerializeObject(transactions));
            }
            catch (Exception ex)
            {
                // Handle exceptions and return a proper error response
                return StatusCode(500, new { message = "An error occurred while fetching transactions.", error = ex.Message });
            }
        }

        #endregion

        #region DeleteTransaction

        [HttpDelete("{id}")]
        public IActionResult DeleteTransaction(int id)
        {
            try
            {
                var isDeleted = _transactionRepository.Delete(id);
                if (!isDeleted)
                {
                    return NotFound(new { message = "Transaction not found." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the transaction.", error = ex.Message });
            }
        }

        #endregion

        #region InsertTransaction

        [HttpPost]
        public IActionResult InsertTransaction([FromBody] TransactionModel transaction)
        {
            try
            {
                if (transaction == null)
                    return BadRequest(new { message = "Invalid transaction data." });

                // Validate the transaction model
                var validationResult = _transactionValidator.Validate(transaction);
                if (!validationResult.IsValid)
                {
                    // Return validation errors
                    var errors = validationResult.Errors.Select(e => new
                    {
                        PropertyName = e.PropertyName,
                        ErrorMessage = e.ErrorMessage
                    });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                bool isInserted = _transactionRepository.Insert(transaction);

                if (isInserted)
                    return Ok(new { message = "Transaction inserted successfully!" });

                return StatusCode(500, new { message = "An error occurred while inserting the transaction." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while inserting the transaction.", error = ex.Message });
            }
        }

        #endregion

        #region UpdateTransaction

        [HttpPut("{id}")]
        public IActionResult UpdateTransaction(int id, [FromBody] TransactionModel transaction)
        {
            try
            {
                if (transaction == null || id != transaction.TransactionID)
                    return BadRequest(new { message = "Transaction ID mismatch or invalid data." });

                // Validate the transaction model
                var validationResult = _transactionValidator.Validate(transaction);
                if (!validationResult.IsValid)
                {
                    // Return validation errors
                    var errors = validationResult.Errors.Select(e => new
                    {
                        PropertyName = e.PropertyName,
                        ErrorMessage = e.ErrorMessage
                    });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var isUpdated = _transactionRepository.Update(transaction);
                if (!isUpdated)
                    return NotFound(new { message = "Transaction not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the transaction.", error = ex.Message });
            }
        }

        #endregion

        #region GetTransaction

        [HttpGet("{id}")]
        public IActionResult GetTransaction(int id)
        {
            try
            {
                var transaction = _transactionRepository.GetTransaction(id);

                if (transaction == null)
                {
                    return NotFound(new { message = "Transaction not found." });
                }

                return Ok(JsonConvert.SerializeObject(transaction));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the transaction.", error = ex.Message });
            }
        }

        #endregion

        #region GetCategory

        [HttpGet("GetCategories/{id}")]
        public IActionResult GetCategories(int id)
        {
            try
            {
                var categories = _transactionRepository.GetCategories(id);

                if (categories == null)
                {
                    return NotFound(new { message = "Categories not found." });
                }

                return Ok(JsonConvert.SerializeObject(categories));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the Categories.", error = ex.Message });
            }
        }

        #endregion

        #region TransactionFilter

        [HttpPost("filter-transactions")]
        public IActionResult GetFilteredTransactions([FromBody] TransactionFilterModel transaction)
        {
            try
            {
                var transactions = _transactionRepository.GetFilteredTransactions(transaction);

                if (transactions == null || !transactions.Any())
                {
                    return NotFound(new { message = "No transactions found matching the criteria." });
                }

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching transactions.", error = ex.Message });
            }
        }

        #endregion


    }
}
