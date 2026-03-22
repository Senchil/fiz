using System;
using System.Collections.Generic;
using System.Text;

namespace fiz.Models
{
    public class StudentRank
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int RankId { get; set; }
        public string SportType { get; set; }
        public DateTime AssignedDate { get; set; }
    }
}