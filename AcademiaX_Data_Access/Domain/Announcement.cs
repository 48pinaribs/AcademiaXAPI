﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Data_Access.Domain
{
	public class Announcement
	{

		public int Id { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public DateTime DatePosted { get; set; }
		public int CreatedBy { get; set; }  // Hangi kullanıcı tarafından oluşturuldu
		public string CreatedByUser { get; set; }  // Kullanıcıyı referans alır
	}
}
