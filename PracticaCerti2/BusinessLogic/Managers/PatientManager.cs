using BusinessLogic.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Managers
{
    public class PatientManager
    {
        string ubicacionRegistro;
        Dictionary<int, PatientEntity> patients;
        string[] tiposSanguineos = {"A+", "A-", "B+", "B-", "O+", "O-"};
        Random rand = new Random();
        private readonly IConfiguration _conf;

        public PatientManager(IConfiguration configuration) 
        {
            _conf = configuration;
            ubicacionRegistro = _conf.GetSection("Paths").GetSection("registroPacientes").Value;
            patients = new Dictionary<int, PatientEntity>();
            ReadBD();
        }
        public void Add(string nombre, string apellido, int ci)
        {
            string tipoSanguineo = tiposSanguineos[rand.Next(6)];
            patients[ci] = new PatientEntity(nombre, apellido, ci, tipoSanguineo);
            WriteBD();
        }
        public void Add(PatientEntity patient)
        {
            patients.Add(patient.CI, patient);
        }
        public void Remove(int ci)
        {
            patients.Remove(ci);
            WriteBD();
        }
        public PatientEntity Get(int ci)
        {
            return patients[ci];
        }
        public Dictionary<int, PatientEntity>.ValueCollection Get()
        {
            return patients.Values;
        }
        public void UpdateNombre(int ci, string nombre) 
        {
            patients[ci].Nombre = nombre;
            WriteBD();
        }
        public void UpdateApellido(int ci, string apellido)
        {
            patients[ci].Apellido = apellido;
            WriteBD();
        }
        private void ReadBD()
        {
            StreamReader reader = new StreamReader(ubicacionRegistro);

            string line = reader.ReadLine();

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                string[] param = line.Split(",");
                string nombre = param[0];
                string apellido = param[1];
                int ci = int.Parse(param[2]);
                string tipoSanguineo = param[3];
                patients.Add(ci, new PatientEntity(nombre, apellido, ci, tipoSanguineo));
            }
            reader.Close();
        }
        private void WriteBD()
        {
            StreamWriter writer = new StreamWriter(ubicacionRegistro);

            writer.WriteLine("Nombre,Apellido,Ci,TipoSanguineo");

            foreach (PatientEntity patient in patients.Values)
            {
                string line = $"{patient.Nombre},{patient.Apellido},{patient.CI},{patient.TipoSanguineo}";
                writer.WriteLine(line);
            }
            writer.Close();
        }
    }
}
