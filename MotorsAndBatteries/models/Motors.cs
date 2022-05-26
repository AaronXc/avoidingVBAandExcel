using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MotorsAndBatteries.models
{
    public class Motors
    {
		public int ID;
		public float mass;
		public float capacity;
		public int number_of_cells;
		public float theoretical_energy;
		public float specific_energy;
		public float max_dischg_current;
		public int c_rating;
		public string model;
		public float price;
		public string currency;
		public string sourced;
	}
}
