using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController: ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name="GetById")]
        public IActionResult GetById(int id)
        {
            var obj = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            obj.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();

            return Ok(obj);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var obj = _context.CelestialObjects.FirstOrDefault(x => x.Name.Equals(name));

            if (obj == null)
            {
                return NotFound();
            }

            obj.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == obj.Id).ToList();

            var nameMatches = _context.CelestialObjects.Where(x => x.Name.Equals(name));

            return Ok(nameMatches);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var objects = _context.CelestialObjects.AsEnumerable();
            foreach (var obj in objects)
            {
                obj.Satellites = _context.CelestialObjects.Where(x => obj.Id == x.Id).ToList();
            }

            return Ok(objects);
        }
    }
}
