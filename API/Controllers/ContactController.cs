using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ContactService _chatService;

        public ContactController(ContactService userService)
        {
            _chatService = userService;
        }

        [HttpGet]
        public ActionResult<List<Contact>> Get() => _chatService.Get();

        [HttpGet("{id:length(24)}", Name = "GetContact")]
        public ActionResult<Contact> Get(string id)
        {
            var message = _chatService.Get(id);

            if (message == null) return NotFound();

            return message;
        }

        [HttpPost]
        public ActionResult<Contact> Create(Contact message)
        {
            _chatService.Create(message);

            return CreatedAtRoute("GetContact", new { id = message.Id.ToString() }, message);
        }



        [HttpPut]
        public ActionResult<Contact> Update(Contact messageIn)
        {
            var message = _chatService.Get(messageIn.Id);

            if (message == null) return NotFound();

            _chatService.Update(messageIn.Id, messageIn);

            return NoContent();
        }


        [HttpDelete]
        public ActionResult<Message> Delete(Message messageIn)
        {
            var message = _chatService.Get(messageIn.Id);

            if (message == null) return NotFound();

            _chatService.Remove(message.Id);

            return NoContent();
        }
    }
}
