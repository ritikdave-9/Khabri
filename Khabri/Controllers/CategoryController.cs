using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data.Entity;
using Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Common.Dtos;
using Common.Utils;

namespace Khabri.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly IBaseService<Category> _categoryService;

        public CategoryController(IBaseService<Category> categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                IEnumerable<Category> categories = await _categoryService.GetAllAsync();



                var result = categories
                            .Select(c => new { CategoryID = c.CategoryID, CategoryName = c.CategoryName , IsActive = c.IsActive })
                            .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                Logger.LogError($"An error occurred while retrieving categories {ex}");
                return StatusCode(500, new ErrorResponseDto{ Message = "An error occurred while retrieving categories."});
            }
        }
        [HttpPut("toggle-active/{id:int}")]
        public async Task<IActionResult> ToggleCategoryActive(int id)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(id);
                if (category == null)
                    return NotFound(new ErrorResponseDto { Message = "Category not found." });

                category.IsActive = !category.IsActive;
                var updated = await _categoryService.UpdateAsync(category);

                if (updated)
                    return Ok(new { category.CategoryID, category.CategoryName, category.IsActive });

                return StatusCode(500, new ErrorResponseDto { Message = "Failed to update category status." });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error toggling category status: {ex}");
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred while updating category status." });
            }
        }

    }
}
