using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class PersonDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Biography { get; set; }
        public string? Quote { get; set; }
        public string? ProfileImagePath { get; set; }
    }
}
