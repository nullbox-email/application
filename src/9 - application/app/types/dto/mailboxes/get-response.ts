export interface GetResponse {
  id: string;
  userId: string;
  routingKey: string;
  name: string;
  domain: string;
  autoCreateAlias: boolean;
  emailAddress: string;
}
