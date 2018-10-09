using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class EventResponse
    {
        public int EventId { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool CanOccurBefore { get; set; }
        public string Source { get; set; }
        public List<EventCategoryResponse> Categories { get; set; } = new List<EventCategoryResponse>();

        public class EventCategoryResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
