export class EventResponse {
  eventId: number;
  eventDate: Date;
  createdDate: Date;
  title: string;
  description: string;
  categories: EventCategory[];
}

export class EventCategory
{
    id: number;
    name: string;
}