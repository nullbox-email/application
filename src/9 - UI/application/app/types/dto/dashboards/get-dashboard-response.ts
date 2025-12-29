export interface TrafficStatisticDto {
  timestamp: string;
  total: number;
  forwarded: number;
  dropped: number;
  quarantined: number;
  delivered: number;
  failed: number;
}

export interface DeliveryActionDto {
  id: string;
  receivedAt: string;
  senderDisplay: string;
  senderAddress: string;
  subject: string;
  messageOutcome: string;
  reason?: string;
  providerStatus?: string;
  hasAttachments?: boolean;
  attachmentCount?: number;
  size?: number;
}

export interface GetDashboardResponse {
  chart: TrafficStatisticDto[];
  messages: DeliveryActionDto[];
}
