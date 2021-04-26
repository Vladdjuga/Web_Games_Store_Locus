using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin")]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationContext _context;
        public ProductController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ResultCollectionDto<ProductDto> GetProducts()
        {
            var selected = _context.Products.Select(el => new ProductDto()
            {
                Name = el.Name,
                Id = el.Id,
                Price = el.Price,
                Image = el.Image,
                Category=new CategoryDto() {Id=el.Category.Id, Name=el.Category.Name}
            }).ToList();
            var res = new ResultCollectionDto<ProductDto>()
            {
                IsSuccess = true,
                Message = "200",
                Data = selected,
            };
            return res;
        }
        [HttpPost]
        public ResultDto AddProduct([FromBody] ProductDto model)
        {
            try
            {
                var product = new Product()
                {
                    Name = model.Name,
                    Price = model.Price,
                    Image = model.Image,
                    Category=_context.Categories.Find(model.Category.Id)
                };
                _context.Products.Add(product);
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
        public ResultCollectionDto<ProductDto> PrepareToUpdate([FromRoute]int id)
        {
            var category = _context.Products.Where(el => el.Id == id).Select(c => new ProductDto()
            {
                Id = c.Id,
                Name = c.Name,
                Category = new CategoryDto() { Id = c.Category.Id, Name = c.Category.Name }
            }).ToList();
            return new ResultCollectionDto<ProductDto>()
            {
                IsSuccess = true,
                Data = category
            };
        }
        [HttpPut]
        public ResultDto EditProduct([FromBody] ProductDto model)
        {
            try
            {
                var product = _context.Products.Find(model.Id);
                product.Name = model.Name;
                product.Price = model.Price;
                product.Image = model.Image;
                product.Category = _context.Categories.Find(model.Category.Id);

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