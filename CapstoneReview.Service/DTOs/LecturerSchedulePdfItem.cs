using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapstoneReview.Service.DTOs
{
    public class LecturerSchedulePdfItem
    {
        public int SlotId { get; set; }
        public int ReviewRound { get; set; }
        public string Room { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string TopicTitle { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
    }
}
