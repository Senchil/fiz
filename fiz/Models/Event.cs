using System;
using System.Collections.Generic;
using System.Text;

namespace fiz.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Organizer { get; set; }
        public string SportType { get; set; }
        public int ParticipantCount { get; set; }
    }
}