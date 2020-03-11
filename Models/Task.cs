using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamAppService.Models
{
    public class Task
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ISet<int> Executors;
        public bool IsCompleted { get; set; }

        public void AddExecutor(int id)
        {
            Executors.Add(id);
        }

        public void RemoveExecutor(int id)
        {
            Executors.Remove(id);
        }
    }
}
