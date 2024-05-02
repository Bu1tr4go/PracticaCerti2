using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Managers;
using BusinessLogic.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PracticaCerti2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        PatientManager manager;
        public PatientController(PatientManager manager)
        {
            this.manager = manager;
        }
        // GET: api/<Patients>
        [HttpGet]
        public IEnumerable<PatientEntity> Get()
        {
            return manager.Get();
        }

        // GET api/<Patients>/5
        [HttpGet("{id}")]
        public PatientEntity Get(int id)
        {
            return manager.Get(id);
        }

        // POST api/<Patients>
        [HttpPost]
        public void Post(string nombre, string apellido, int ci)
        {
            manager.Add(nombre, apellido, ci);
        }

        // PUT api/<Patients>/5
        [HttpPut("nombre/{id}")]
        public void PutName(int id, string nombre)
        {
            manager.UpdateNombre(id, nombre);
        }

        [HttpPut("apellido/{id}")]
        public void PutLastName(int id, string apellido)
        {
            manager.UpdateApellido(id, apellido);
        }

        // DELETE api/<Patients>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            manager.Remove(id);
        }
    }
}
