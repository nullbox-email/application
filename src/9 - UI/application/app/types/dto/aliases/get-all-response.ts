export interface GetAllResponse {
  id: string;
  mailboxId: string;
  name: string;
  localPart: string;
  isEnabled: boolean;
  directPassthrough: boolean;
  learningMode: boolean;
}
