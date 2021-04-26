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
    public class TagController : ControllerBase
    {
        private readonly ApplicationContext _context;
        public TagController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ResultCollectionDto<TagDto> GetTags()
        {
            var selected = _context.Tags.Select(el => new TagDto()
            {
                Name = el.Name,
                Id = el.Id
            }).ToList();
            var res = new ResultCollectionDto<TagDto>()
            {
                IsSuccess = true,
                Message = "200",
                Data = selected,
            };
            return res;
        }
        [HttpPost]
        public ResultDto AddTag([FromBody] TagDto model)
        {
            try
            {
                var tag = new Tag()
                {
                    Name = model.Name
                };
                _context.Tags.Add(tag);
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
        public ResultCollectionDto<TagDto> PrepareToUpdate([FromRoute]int id)
        {
            var category = _context.Products.Where(el => el.Id == id).Select(c => new TagDto()
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
            return new ResultCollectionDto<TagDto>()
            {
                IsSuccess = true,
                Data = category
            };
        }
        [HttpPut]
        public ResultDto EditTag([FromBody] TagDto model)
        {
            try
            {
                var tag = _context.Tags.Find(model.Id);
                tag.Name = model.Name;

                _context.Tags.Add(tag);
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