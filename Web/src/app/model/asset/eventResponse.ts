export class EventResponse {
  id: number;
  title: string;
  description: string;
  eventDate: Date;
  createdDate: Date;
  categories: EventCategory[];
}

export class EventCategory
{
    id: number;
    name: string;
}