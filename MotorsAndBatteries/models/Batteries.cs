using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MotorsAndBatteries.models
{
    public class Batteries
    {
		public int ID;
		public int expected_drone_mass;
		public float expected_quarter_total_drone_thrust;
		public string model;
		public float four_motor_mass;
		public float single_motor_thrust;
		public int throttle;
		public float amperage;
		public float voltage;
		public float power_drawn;
		public float efficiency;
		public string propeller_model;
		public float supported_mass;
		public float thirteen_minute_energy;
	}
}
