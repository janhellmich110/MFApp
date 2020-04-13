using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MFApp.Models
{
    public class Tee
    {
        [PrimaryKey]
        public int Id { get; set; }

        public int Length { get; set; }
        public int Hcp { get; set; }
        public int Par { get; set; }
        public int Name { get; set; }
        public string Textname { get; set; }

        public int CourseId { get; set; }

    }
}
