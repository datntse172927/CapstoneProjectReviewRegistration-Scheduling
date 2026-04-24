using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapstoneReview.Service.Interfaces
{
    public interface ILecturerSchedulePdfService
    {
        Task<byte[]> ExportLecturerSchedulePdfAsync(int lecturerId, int reviewRound);
    }
}
