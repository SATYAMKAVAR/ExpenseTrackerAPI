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
    public class CategoryController : ControllerBase
    {
        #region CategoryRepository ,CategoryValidator
        private readonly CategoryRepository _categoryRepository;
        private readonly IValidator<CategoryModel> _categoryValidator;

        public CategoryController(CategoryRepository categoryRepository, IValidator<CategoryModel> categoryValidator)
        {
            _categoryRepository = categoryRepository;
            _categoryValidator = categoryValidator;
        }
        #endregion

        #region GetAllCategories

        [HttpGet("/api/Categories/{userID}")]
        public IActionResult GetAllCategories(int userID)
        {
            try
            {
                var categories = _categoryRepository.GetAllCategories(userID);
                return Ok(JsonConvert.SerializeObject(categories));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching Categories.", error = ex.Message });
            }
        }

        #endregion

        #region DeleteCategory

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                var isDeleted = _categoryRepository.Delete(id);
                if (!isDeleted)
                {
                    return NotFound(new { message = "Category not found." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the category.", error = ex.Message });
            }
        }

        #endregion

        #region InsertCategory

        [HttpPost]
        public async Task<IActionResult> InsertCategory([FromBody] CategoryModel category)
        {
            try
            {
                if (category == null)
                    return BadRequest(new { message = "Invalid category data." });

                // Validate the category model
                var validationResult = await _categoryValidator.ValidateAsync(category);

                if (!validationResult.IsValid)
                {
                    // Return validation errors directly in the response
                    var errors = validationResult.Errors.Select(e => new
                    {
                        PropertyName = e.PropertyName,
                        ErrorMessage = e.ErrorMessage
                    });

                    return BadRequest(new { message = "Validation failed", errors });
                }

                bool isInserted = _categoryRepository.Insert(category);

                if (isInserted)
                    return Ok(new { Message = "Category inserted successfully!" });

                return StatusCode(500, new { message = "An error occurred while inserting the category." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while inserting the category.", error = ex.Message });
            }
        }

        #endregion

        #region UpdateCategory

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryModel category)
        {
            try
            {
                if (category == null || id != category.CategoryID)
                    return BadRequest(new { message = "Category ID mismatch or invalid data." });

                // Validate the category model
                var validationResult = await _categoryValidator.ValidateAsync(category);

                if (!validationResult.IsValid)
                {
                    // Return validation errors directly in the response
                    var errors = validationResult.Errors.Select(e => new
                    {
                        PropertyName = e.PropertyName,
                        ErrorMessage = e.ErrorMessage
                    });

                    return BadRequest(new { message = "Validation failed", errors });
                }

                var isUpdated = _categoryRepository.Update(category);
                if (!isUpdated)
                    return NotFound(new { message = "Category not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the category.", error = ex.Message });
            }
        }

        #endregion

        #region GetCategory

        [HttpGet("{id}")]
        public IActionResult GetCategory(int id)
        {
            try
            {
                var category = _categoryRepository.GetCategory(id);
                if (category == null)
                {
                    return NotFound(new { message = "Category not found." });
                }
                return Ok(JsonConvert.SerializeObject(category));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching category.", error = ex.Message });
            }
        }

        #endregion
    }
}