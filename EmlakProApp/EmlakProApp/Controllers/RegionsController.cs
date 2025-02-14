using EmlakProApp.Data;
using EmlakProApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmlakProApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RegionsController : ControllerBase
	{
		private readonly AppDbContext _appDbContext;
        public RegionsController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Region>>> GetRegions()
		{
			//return await _appDbContext.region.ToListAsync();
			return Ok();
		}



		//[HttpGet("all")]
		//public async Task<ActionResult<IEnumerable<Region>>> GetAll()
		//{
		//	return await _appDbContext.region.ToListAsync();
		//}
	}
}
