using System;

namespace Azuretodo.Common.Models
{
    public class Todo
    {
        public DateTime CreatedTime { get; set; }
        public bool IsCompleted { get; set; }
        public string TaskDescription { get; set; }
    }
}
