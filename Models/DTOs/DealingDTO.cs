﻿using System.ComponentModel.DataAnnotations;

namespace Qurabani.com_Server.Models.DTOs
{
	public class DealingDTO
	{
		public string Name { get; set; }
		public string Contact { get; set; }
		public string EmergencyContact { get; set; }
		public string Address { get; set; }
		public string Nic { get; set; }
		public int AdId { get; set; }
		public int PartId { get; set; }
		public int QurbaniDay { get; set; }
		public string Description { get; set; }
	}
}