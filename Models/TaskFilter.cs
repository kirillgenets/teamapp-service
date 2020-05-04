using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamAppService.Models
{
    public class TaskFilter
    {
        public DateTime? date { get; set; }
        public long? id { get; set; }
        public string? title { get; set; }
        public string? category { get; set; }
        public long? assigneeId { get; set; }
    }
}
