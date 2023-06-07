using System;

namespace CleanArc.Application.Models
{
    public class ToDoModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ChallengeId { get; set; }
        public bool Notify { get; set; }
    }
}
