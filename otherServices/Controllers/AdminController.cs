
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using otherServices.Models;
using otherServices.Services;
using WebAPIDotNet.Services;
namespace otherServices.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/admin/")]
    [ApiController]
    public class AdminController :ControllerBase
    {

        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            this._adminService = adminService;
        }


        [HttpGet("all-user/")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _adminService.GetUsers();  
            if (result == null || !result.Any())
            {
                return NotFound("Not found");
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpGet("waitingLandlords")]
        public async Task<IActionResult> GetWaitingLandlord()
        {
            var result = await _adminService.GetWaitingLandlord();
            if (result == null || !result.Any())
            {
                return NotFound();
            }
            else
            {
                return Ok(result);
            }
        }
        [HttpGet("landlord-status/{landlordId}")]
        public async Task<IActionResult> GetLanglordStatus(long landlordId)
        {
            var result = await _adminService.GetLandlordStatus(landlordId);
            if (result == null || !result.Any())
            {
                return NotFound();
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpPut("accept-waiting-landlord/{id}")]
        public async Task<IActionResult> AcceptUser(int id)
        {
            try
            {
                var acceptedUser = await _adminService.AcceptUser(id);
                return Ok(acceptedUser); 
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
        }

        [HttpPut("reject-waiting-landlord/{id}")]
        public async Task<IActionResult> RejectUser(int id)
        {
            try
            {
                var rejectedUser = await _adminService.RejectUser(id);
                return Ok(rejectedUser); 
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
        }


        [HttpGet("waitingPosts")]
        public async Task<IActionResult> GetWaitingPosts()
        {
            var result = await _adminService.GetWaitingPosts();
            if (result == null || !result.Any())
            {
                return NotFound();
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpPut("accept-post/{id}")]
        public async Task<IActionResult> AcceptPost(int id)
        {
            try
            {
                var acceptedPost = await _adminService.AcceptPost(id);
                return Ok(acceptedPost); 
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
        }

        [HttpPut("reject-post/{id}")]
        public async Task<IActionResult> RejectPost(int id)
        {
            try
            {
                var rejectedPost = await _adminService.RejectPost(id);
                return Ok(rejectedPost); 
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
        }
        
        
    }
}
