export interface DeliveryActionDto {
 id: string;
 receivedAt: string;
 senderDisplay: string;
 senderDomain: string;
 subject: string;
 messageOutcome: string;
 providerStatus?: string;
 hasAttachments?: boolean;
 attachmentsCount?: number;
}

export interface TrafficStatisticDto {
  timestamp: string;
  total: number;
  forwarded: number;
  dropped: number;
  quarantined: number;
  delivered: number;
  failed: number;
}

export interface GetDashboardResponse {
  chart: TrafficStatisticDto[];
  messages: DeliveryActionDto[];
}
