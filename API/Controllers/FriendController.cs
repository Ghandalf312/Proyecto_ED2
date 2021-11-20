using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using API.Models;

using API.Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly FriendService _chatService;

        public FriendController(FriendService userService)
        {
            _chatService = userService;
        }

        [HttpGet]
        public ActionResult<List<Friend>> Get() => _chatService.Get();

        [HttpGet("{id:length(24)}", Name = "GetFriend")]
        public ActionResult<Friend> Get(string id)
        {
            var message = _chatService.Get(id);

            if (message == null) return NotFound();

            return message;
        }

        [HttpPost]
        public ActionResult<Friend> Create(Friend message)
        {
            _chatService.Create(message);

            return CreatedAtRoute("GetFriend", new { id = message.Id.ToString() }, message);
        }


        //[HttpPut("{id:length(24)}")]
        [HttpPut]
        public ActionResult<Friend> Update(Friend messageIn)
        {
            var message = _chatService.Get(messageIn.Id);

            if (message == null) return NotFound();

            _chatService.Update(messageIn.Id, messageIn);

            return NoContent();
        }

        //[HttpDelete("{id:length(24)}")]
        [HttpDelete]
        public ActionResult<Friend> Delete(Friend messageIn)
        {
            var message = _chatService.Get(messageIn.Id);

            if (message == null) return NotFound();

            _chatService.Remove(message.Id);

            return NoContent();
        }
    }
}
