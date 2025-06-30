using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data.Entity;
using Service.Interfaces;
using Microsoft.AspNetCore.Authorization;

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
                IEnumerable<Category> categories = await _categoryService.FindAllAsync(c => c.IsActive);



                var result = categories
                            .Select(c => new { CategoryID = c.CategoryID, CategoryName = c.CategoryName })
                            .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving categories.", Details = ex.Message });
            }
        }
    }
}
