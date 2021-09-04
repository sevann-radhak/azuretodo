﻿using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Azuretodo.Functions.Entities
{
    public class TodoEntity : TableEntity
    {
        public DateTime CreatedTime { get; set; }
        public bool IsCompleted { get; set; }
        public string TaskDescription { get; set; }
    }
}
