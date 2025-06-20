using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data.Entity;
using Service.Interfaces;

namespace Khabri.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IBaseService<Category> _categoryService;

        public CategoryController(IBaseService<Category> categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                IEnumerable<Category> categories = await _categoryService.GetAllAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving categories.", Details = ex.Message });
            }
        }
    }
}
