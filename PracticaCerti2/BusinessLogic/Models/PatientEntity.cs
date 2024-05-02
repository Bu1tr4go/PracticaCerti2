using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class PatientEntity
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int CI { get; set; }
        public string TipoSanguineo {  get; set; }
        public PatientEntity(string nombre, string apellido, int ci, string tipoSanguineo) 
        {
            Nombre = nombre;
            Apellido = apellido;
            CI = ci;
            TipoSanguineo = tipoSanguineo;
        }
    }
}
