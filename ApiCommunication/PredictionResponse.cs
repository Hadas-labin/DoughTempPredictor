using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.ApiCommunication
{
    public class PredictionResponse
    {
        public double IceAmount { get; set; }
        public double? OilAmount { get; set; }
        public string? ErrorMessage { get; set; } = "";
    }
}
