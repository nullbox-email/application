export type DashboardType = "Hourly" | "Daily";

export interface GetDashboardRequest {
  aliasId?: string;
  mailboxId?: string;
  number: number;
  type: DashboardType;
}
