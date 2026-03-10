using System;
using System.Collections.Generic;
using System.Text;

namespace fiz.Models
{
    public class Participation
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string StudentName { get; set; }
        public string Result { get; set; }
        public string Award { get; set; }
        public string Rank { get; set; }
        public string AddedBy { get; set; }
        public string Date { get; set; }
    }
}
