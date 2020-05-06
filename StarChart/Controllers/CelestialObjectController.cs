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

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject co)
        {
            _context.Add(co);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = co.Id }, co);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject co)
        {
            var currObj = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);

            if (currObj == null)
            { 
                return NotFound();
            }

            currObj.Name = co.Name;
            currObj.OrbitalPeriod = co.OrbitalPeriod;
            currObj.OrbitedObjectId = co.OrbitedObjectId;

            _context.Update(currObj);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var currObj = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);

            if (currObj == null)
            {
                return NotFound();
            }

            currObj.Name = name;

            _context.Update(currObj);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var matchingObjs = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id);

            if (!matchingObjs.Any())
            {
                return NotFound();
            }

            _context.RemoveRange(matchingObjs);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
