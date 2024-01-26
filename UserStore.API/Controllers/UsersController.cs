﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using UserStore.API.Contracts;
using UserStore.Core.Abstractions;
using UserStore.Core.Models;


namespace UserStore.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _userService;

        public UsersController(IUsersService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UsersResponse>>> GetUsers()
        {

            var users = await _userService.GetAllUsers();

            var response = users.Select(u => new UsersResponse(u.Id, u.Еmail, u.Password, u.Token));

            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<ActionResult<List<UsersRequest>>> CreateUsers([FromBody] UsersRequest request)
        {
            if (_userService.FindExistingUser(request.email))
            {
                return BadRequest("User already exists");
            }

            var (user, error) = UserStore.Core.Models.User.Create(
                request.email,
                request.password);

            if(!string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }

            var userToken = await _userService.CreateUser(user);

            string jsonUserToken = JsonSerializer.Serialize(userToken);

            return Ok(jsonUserToken);
        }
        [HttpPost("Login")]
        public async Task<ActionResult<List<UsersRequest>>> LoginUsers([FromBody] UsersRequestLogin request)
        {
            var (user, error) = UserStore.Core.Models.User.Create(
                request.token
                );

            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }

            string loginResult = _userService.LoginUser(user); //repository
            Console.WriteLine(loginResult);

            if(!loginResult.Contains("-"))
            {
                Console.WriteLine("loginResult " + loginResult);
                Console.WriteLine("error error");
                return BadRequest(loginResult);
            }

            Console.WriteLine("done done");
            Guid id = Guid.Parse(loginResult);

            var findUser = _userService.FindById(id);

            return Ok(findUser);

        }

        [HttpPut("{id:Guid}")]
        public async Task<ActionResult<Guid>> UpdateUser(Guid id, [FromBody] UsersRequest request)
        {
            var userUd = await _userService.UpdateUser(
                id,
                request.email,
                request.password);

            return Ok(userUd);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult<Guid>> DeleteUser(Guid id)
        {
            return Ok(await _userService.DeleteUser(id));
        }
    }
}
