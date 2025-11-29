using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using otherServices.Models;
using otherServices.Models.DTOs;
using otherServices.Services;
using WebAPIDotNet.DTOs;
using WebAPIDotNet.Services;


namespace otherServices.Controllers
{
    //[Authorize(Roles = "tenant")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class TenantController:ControllerBase
    {
        private readonly ITenantService _tenantService;
        public TenantController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }


        [HttpPost("submit-proposal/{PostId}")]
        public async Task<IActionResult> SubmitProposal(long PostId, [FromForm] SubmitProposalDto form)
        {
            try
            {
                var result = await _tenantService.SubmitProposalAsync(PostId,form);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("my-proposals/{tenantId}")]
        public async Task<IActionResult> GetProposalsForLandlord(long tenantId)
        {
            try
            {
                var proposals = await _tenantService.GetTenantProposalsAsync(tenantId);
                if (proposals == null)
                {
                    return NotFound(new { message = "No proposals found for this tenant" });
                }
                return Ok(proposals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving proposals", error = ex.Message });
            }
        }


        [HttpPut("edit-proposal/{proposalId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditProposal(long proposalId, [FromForm] ProposalEditDto updated)
        {
            var success = await _tenantService.EditProposalAsync(proposalId, updated);
            if (!success) return NotFound("Proposal not found");
            return Ok("Proposal updated");
        }


        [HttpDelete("cancel-proposal/{proposalId}")]
        public async Task<IActionResult> DeleteProposal(long proposalId)
        {
            var success = await _tenantService.DeleteProposalAsync(proposalId);
            if (!success) return NotFound("Proposal not found");
            return Ok("Proposal deleted");
        }



        [HttpGet("all-posts/")]
        public async Task<IActionResult> GetPost()
        {
            var result = await _tenantService.GetPosts();
            if (result == null || !result.Any())
            {
                return NotFound("Not found");
            }
            else
            {
                return Ok(result);
            }
        }



        [HttpPost("{tenantId}/save-post/{postId}")]
        public async Task<IActionResult> SavePost(
                [FromRoute] long postId,
                long tenantId)
        {
            try
            {
                var result = await _tenantService.Save_Post(tenantId, postId);

                if (!result)
                    return BadRequest(new
                    {
                        success = false,
                        message = "Post already saved or invalid data"
                    });

                return Ok(new
                {
                    success = true,
                    message = "Post saved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{userId}/cancel-save/{postId}")]
        public async Task<IActionResult> cancelSave(long userId,long postId)
        {
            var success = await _tenantService.cancelSave(userId,postId);
            if (!success) return NotFound("post not found");
            return Ok("post deleted");
        }


        [HttpGet("My-saved-posts/{tenantId}")]
        public async Task<IActionResult> GetMySavedPosts(long tenantId)
        {
            var result = await _tenantService.GetMySavedPosts(tenantId);
            if (result == null || !result.Any())
            {
                return NotFound("Not found");
            }
            else
            {
                return Ok(result);
            }
        }
        
    }
}
