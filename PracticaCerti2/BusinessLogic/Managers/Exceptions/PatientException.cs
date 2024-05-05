using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Managers.Exceptions
{
    public class PatientException : Exception
    {
        public PatientException() { }
        public PatientException(string message) : base(message) { }

        public string GetMensajeforLogs(string method)
        {
            return $"{method} | Exception: {Message}";
        }
    }
}
