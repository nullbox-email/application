import type { GetDashboardRequest } from "~/types/dto/dashboards/get-dashboard-request";
import type { GetDashboardResponse } from "~/types/dto/dashboards/get-dashboard-response";

export function useDashboards() {

    const getDashboard = async (version: string, request: GetDashboardRequest, options?: any) => {
        const result = await useApi<GetDashboardResponse>(
            '/:version/dashboard',
            {
                ...(options || {}),
                method: 'GET',
                params: {
                    version: version
                },
                query: {
                    aliasId: request.aliasId,
                    mailboxId: request.mailboxId,
                    number: request.number,
                    type: request.type
                },
                scopes: ['nullbox.dashboard.read-all'],
            }
        );
        return result.data.value;
    }

    return {
        getDashboard,
    }
}
