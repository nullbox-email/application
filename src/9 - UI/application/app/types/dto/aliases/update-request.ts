export interface UpdateRequest {
  id: string;
  mailboxId: string;
  name: string;
  isEnabled: boolean;
  directPassthrough: boolean;
  learningMode: boolean;
}
