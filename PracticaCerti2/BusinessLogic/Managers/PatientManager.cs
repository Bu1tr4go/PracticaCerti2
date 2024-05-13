using BusinessLogic.Managers.Exceptions;
using BusinessLogic.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
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
        public async void Add(string nombre, string apellido, int ci)
        {
            if (patients.ContainsKey(ci))
            {
                PatientException ex = new PatientException("CI ya registrado");
                Log.Error(ex.GetMensajeforLogs("Add patient"));
                throw ex;
            }
            else
            {
                string tipoSanguineo = tiposSanguineos[rand.Next(6)];
                string codigo = await ObtenerCodigo(nombre, apellido, ci);
                patients[ci] = new PatientEntity(nombre, apellido, ci, tipoSanguineo, codigo);
                WriteBD();
            }
        }
        public void Add(PatientEntity patient)
        {
            patients.Add(patient.CI, patient);
        }
        public void Remove(int ci)
        {
            if (patients.ContainsKey(ci))
            {
                patients.Remove(ci);
                WriteBD();
            }
            else
            {
                Log.Error("Patient not found");
            }
        }
        public PatientEntity Get(int ci)
        {
            try
            {
                return patients[ci];
            }
            catch (Exception e)
            {
                PatientException ex = new PatientException(e.Message);
                Log.Error(ex.GetMensajeforLogs("Get by CI"));
                Log.Error("Patient not found");
                throw ex;
            }
        }

        public Dictionary<int, PatientEntity>.ValueCollection Get()
        {
            return patients.Values;
        }
        public void UpdateNombre(int ci, string nombre) 
        {
            try
            {
                patients[ci].Nombre = nombre;
                WriteBD();
            }
            catch(Exception e)
            {
                PatientException ex = new PatientException(e.Message);
                Log.Error(ex.GetMensajeforLogs("Update Name by CI"));
                Log.Error("Patient not found");
                throw ex;
            }
        }
        public void UpdateApellido(int ci, string apellido)
        {
            try
            {
                patients[ci].Apellido = apellido;
                WriteBD();
            }
            catch( Exception e )
            {
                PatientException ex = new PatientException(e.Message);
                Log.Error(ex.GetMensajeforLogs("Update LastName by CI"));
                Log.Error("Patient not found");
                throw ex;
            }
        }
        private async void ReadBD()
        {
            StreamReader reader = new StreamReader(ubicacionRegistro);

            string line = reader.ReadLine();

            bool guardar = false;

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                string[] param = line.Split(",");
                string nombre = param[0];
                string apellido = param[1];
                int ci = int.Parse(param[2]);
                string tipoSanguineo = param[3];
                string codigo;
                if (param.Length > 4)
                {
                    codigo = param[4];
                }
                else
                {
                    codigo = await ObtenerCodigo(nombre, apellido, ci);
                    guardar = true;
                }
                patients.Add(ci, new PatientEntity(nombre, apellido, ci, tipoSanguineo, codigo));
            }
            reader.Close();

            if (guardar)
            {
                WriteBD();
            }
        }
        private async void WriteBD()
        {
            StreamWriter writer = new StreamWriter(ubicacionRegistro);

            writer.WriteLine("Nombre,Apellido,Ci,TipoSanguineo,Codigo");

            foreach (PatientEntity patient in patients.Values)
            {
                string line = $"{patient.Nombre},{patient.Apellido},{patient.CI},{patient.TipoSanguineo},{patient.Codigo}";
                writer.WriteLine(line);
            }
            writer.Close();
        }
        public async Task<string> ObtenerCodigo(string nombre, string apellido, int ci)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{_conf.GetSection("Paths").GetSection("api3").Value}");
                    string url = $"{_conf.GetSection("Paths").GetSection("ruta").Value}{nombre}/{apellido}/{ci}";
                    HttpResponseMessage response = await client.GetAsync(url);
                    Log.Information("Obteniendo Codigo");

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("PATIENT CODE recibido: " + responseBody);
                        return responseBody;
                    }
                    else
                    {
                        Log.Error("La solicitud HTTP no fue exitosa. Código de estado: " + response.StatusCode);
                        PatientException ex = new PatientException("Error en la solicitud http");
                        throw ex;
                    }
                }
            }
            catch (Exception e)
            {
                PatientException ex = new PatientException(e.Message);
                Log.Error(ex.GetMensajeforLogs("Error al realizar la solicitud HTTP: "));
                throw ex;
            }
        }

    }
}
