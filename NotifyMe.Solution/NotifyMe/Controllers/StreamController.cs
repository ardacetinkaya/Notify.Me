using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NotifyMe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamController : ControllerBase
    {
        private HttpClient _client;

        public StreamController(){
            
            _client = new HttpClient();
        }

        [HttpGet]
        public async Task<FileStreamResult> Get()
        {
            var video =$"https://notifyminepla.azurewebsites.net/video/birdahacalismayacagim.mp4";
            var stream =  await _client.GetStreamAsync(video);

            return new FileStreamResult(stream,"video/mp4");
        }

    }
}
