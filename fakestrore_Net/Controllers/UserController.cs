﻿using fakestrore_Net.DTOs.OrderDTO;
using fakestrore_Net.DTOs.UserDTO;
using fakestrore_Net.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonException = Newtonsoft.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace fakestrore_Net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer, Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("order")]
        public async Task<ActionResult<List<Product>>> AddOrder(OrderCreateDTO request)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                MaxDepth = 32,
                IgnoreNullValues = true,
                PropertyNameCaseInsensitive = true
            };

            try
            {
                var result = await _userService.AddOrder(request);
                if (result == null)
                {
                    return BadRequest("Can't add product");
                }

                var json = JsonSerializer.Serialize(result, options);

                // Tiếp tục xử lý JSON hoặc trả về JSON nếu cần thiết
                // ...

                return Ok("Success!");
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Lỗi serialize JSON: " + ex.Message);
                return BadRequest("Can't serialize result");
            }
        }
        [HttpGet("me")]
        public async Task<ActionResult<UserGetDTO>> Information()
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                MaxDepth = 32,
                IgnoreNullValues = true,
                PropertyNameCaseInsensitive = true
            };

            try
            {
                var result = await _userService.Information();
                if (result.Result is BadRequestResult)
                {
                    return BadRequest("Không tìm thấy");
                }
                else if (result.Result is NotFoundResult)
                {
                    return NotFound("Người dùng không tồn tại");
                }
                else
                {
                    var settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    };

                    var json = JsonConvert.SerializeObject(result, Formatting.None, settings);
                    JObject jObject = JObject.Parse(json);
                    json = jObject.ToString();
                    return Ok(json);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Lỗi serialize JSON: " + ex.Message);
                return BadRequest("Can't serialize result");
            }
        }
    }
}
