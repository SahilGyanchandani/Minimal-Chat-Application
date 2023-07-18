using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_Chat_Application.Models;
using Minimal_Chat_Application.ParameterModels;
using Minimal_Chat_Application.Response_Models;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.Utilities;
using System.Security.Claims;

namespace Minimal_Chat_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {

        private readonly AppDbContext _Context;
        public MessageController(AppDbContext dbContext)
        {
            _Context = dbContext;
        }
        [HttpGet,Authorize]
        public async Task<IActionResult> GetConversationHistory(int userId,DateTime? before =null,int count=20,string sort="asc")
        {
            string currentEmail=GetCurrentEmail();
            var sender= await _Context.UserRegistrations.FirstOrDefaultAsync(ur => ur.Email==currentEmail);
            if (sender==null)
            {
                return Unauthorized();
            }
           

            var messages=await _Context.Messages
                .Where(m => (m.UserID == userId && m.ReceiverID==sender.UserID || (m.UserID==sender.UserID && m.ReceiverID==userId))
                && (before == null || m.Timestamp<before))                                                                                                                                                              
                .OrderBy(m =>m.Timestamp)
                .ToListAsync();

            // Sort the messages based on the sort parameter
            if (sort.ToLower() == "desc")
            {
                messages = messages.OrderByDescending(m => m.Timestamp).ToList();
            }
            else
            {
                messages = messages.OrderBy(m => m.Timestamp).ToList();
            }

            // Apply the count parameter
            messages = messages.Take(count).ToList();

            // Map the messages to the response model
            var response = messages.Select(m => new
            {
                id = m.MessageID,
                userId = m.UserID,
                receiverID = m.ReceiverID,
                content = m.Content,
                timestamp = m.Timestamp
            });


            return Ok(new {messages=response});
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> SendMessage([FromBody] ReceiveMessage msg)
        {
            try
            {

                // Get the current user's email
                string currentEmail = GetCurrentEmail();

                // Retrieve the sender from the database
                var sender = await _Context.UserRegistrations.FirstOrDefaultAsync(ur => ur.Email == currentEmail);
                if (sender == null)
                {
                    return Unauthorized();
                }
                var message = new Message
                {
                    UserID = sender.UserID,
                    ReceiverID = msg.ReceiverID,
                    Content = msg.Content,
                    Timestamp = DateTime.Now,
                    IsActive= true
                };

                _Context.Messages.Add(message);
                await _Context.SaveChangesAsync();
                var response = new MessageResponse
                {
                    Content = message.Content,
                    Timestamp = message.Timestamp
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }

        [HttpPut("{msgId}"), Authorize]
        public async Task<ActionResult> MessageEdit(int msgId, [FromBody] EditMessage edit)
        {
            // Get the current user's email
            string currentEmail = GetCurrentEmail();

            // Retrieve the message from the database
            var message = await _Context.Messages.FindAsync(msgId);

            // Check if the message exists
            if (message == null)
            {
                return NotFound(new { error = "Message Not Found" });
            }

            // Check if the current user is the sender of the message
            var sender = await _Context.UserRegistrations.FirstOrDefaultAsync(u => u.Email == currentEmail);
            if (sender == null || sender.UserID != message.UserID)
            {
                return Unauthorized();
            }

            // Update the message content
            message.Content = edit.Content;

            // Save changes to the database
            await _Context.SaveChangesAsync();

            return Ok();
        }
        [HttpDelete("{msgId}"),Authorize]
        public async Task<IActionResult> DeleteMessage(int msgId)
        {
            // Get the current user's email
            string currentEmail=GetCurrentEmail();

            // Retrieve the message from the database
            var message = await _Context.Messages.FindAsync(msgId);

            // Check if the message exists
            if (message == null)
            {
                return NotFound();
            }

            // Check if the current user is the sender of the message
            var sender = await _Context.UserRegistrations.FirstOrDefaultAsync(ur => ur.Email == currentEmail);
            if(sender==null || sender.UserID != message.UserID)
            { 
                return Unauthorized(); 
            }
            // Remove the message from the database
            _Context.Messages.Remove(message);
            await _Context.SaveChangesAsync();

            return Ok();
        }

        private string GetCurrentEmail()
        {
            // Get the current user's claims principal
            var claimsPrincipal = HttpContext.User;

            // Get the email claim
            var emailClaim = claimsPrincipal.FindFirst(ClaimTypes.Email);

            return emailClaim?.Value;
        }
       



    }
}
