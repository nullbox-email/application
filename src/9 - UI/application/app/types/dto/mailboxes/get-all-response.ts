export interface GetAllResponse {
  id: string;
  name: string;
  routingKey: string;
  domain: string;
  aliases: AliasDto[];
  autoCreateAlias: boolean;
  emailAddress: string;
}
