using Newtonsoft.Json;
using System;

namespace VulnerableNotesApp.Model
{
    public class Note
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public bool Important { get; set; }
        [JsonIgnore]
        public string UserId { get; set; }
    }
}
