using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web_Games_Store_Locus.Models;
using Web_Games_Store_Locus.Models.Dto;
using Web_Games_Store_Locus.Models.Entities;
using Web_Games_Store_Locus.Models.Results;

namespace Web_Games_Store_Locus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationContext _context;
        public CategoryController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ResultCollectionDto<CategoryDto> GetCategorys()
        {
            var selected = _context.Category.Select(el => new CategoryDto()
            {
                Name = el.Name,
                Id = el.Id
            }).ToList();
            var res = new ResultCollectionDto<CategoryDto>()
            {
                IsSuccess = true,
                Message = "200",
                Data = selected,
            };
            return res;
        }
        [HttpPost]
        public ResultDto AddCategory([FromBody] CategoryDto model)
        {
            try
            {
                var category = new Category()
                {
                    Name = model.Name
                };
                _context.Category.Add(category);
                _context.SaveChanges();
                var res = new ResultDto()
                {
                    IsSuccess = true,
                    Message = "200"
                };
                return res;
            }
            catch (Exception ex)
            {
                var err = new ResultDto()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
                return err;
            }
        }
        [HttpGet]
        [Route("prepare/{id}")]
        public ResultCollectionDto<CategoryDto> PrepareToUpdate([FromRoute] int id)
        {
            var category = _context.Category.Where(el => el.Id == id).Select(c => new CategoryDto()
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
            return new ResultCollectionDto<CategoryDto>()
            {
                IsSuccess = true,
                Data = category
            };
        }
        [HttpPut]
        public ResultDto EditProduct([FromBody] CategoryDto model)
        {
            try
            {
                var category = _context.Category.Find(model.Id);
                category.Name = model.Name;

                _context.Products.Add(category);
                _context.SaveChanges();
                var res = new ResultDto()
                {
                    IsSuccess = true,
                    Message = "200"
                };
                return res;
            }
            catch (Exception ex)
            {
                var err = new ResultDto()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
                return err;
            }
        }
    }
}
