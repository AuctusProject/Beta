export class EventResponse {
  eventId: number;
  title: string;
  description: string;
  eventDate: Date;
  creationDate: Date;
  canOccurBefore: boolean;
  source: string;
  categories: EventCategory[];
}

export class EventCategory
{
    id: number;
    name: string;
}