using System.Collections.Generic;

namespace TeamAppService.Models
{
    public class Task
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ISet<int> Executors;
        public bool IsCompleted { get; set; }
        public long AssigneeID { get; set; }

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
