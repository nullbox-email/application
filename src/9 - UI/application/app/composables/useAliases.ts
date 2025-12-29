import type { CreateRequest } from "~/types/dto/aliases/create-request";
import type { CreateResponse } from "~/types/dto/aliases/create-response";
import type { UpdateRequest } from "~/types/dto/aliases/update-request";
import type { GetRequest } from "~/types/dto/aliases/get-request";
import type { GetResponse } from "~/types/dto/aliases/get-response";
import type { GetAllRequest } from "~/types/dto/aliases/get-all-request";
import type { GetAllResponse } from "~/types/dto/aliases/get-all-response";

export function useAliases() {

    const create = async (version: string, mailboxId: string, body: CreateRequest, options?: any) => {
        const result = await useApi<CreateResponse>(
            '/:version/mailboxes/:mailboxId/aliases',
            {
                ...(options || {}),
                method: 'POST',
                body,
                params: {
                    version: version,
                    mailboxId: mailboxId
                },
                scopes: ['nullbox.alias.create'],
            }
        );
        return result.data.value;
    }
    const get = async (version: string, request: GetRequest, options?: any) => {
        const result = await useApi<GetResponse>(
            '/:version/mailboxes/:mailboxId/aliases/:id',
            {
                ...(options || {}),
                method: 'GET',
                params: {
                    version: version,
                    id: request.id,
                    mailboxId: request.mailboxId
                },
                scopes: ['nullbox.alias.read'],
            }
        );
        return result.data.value;
    }
    const getAll = async (version: string, request: GetAllRequest, options?: any) => {
        const result = await useApi<GetAllResponse>(
            '/:version/mailboxes/:mailboxId/aliases',
            {
                ...(options || {}),
                method: 'GET',
                params: {
                    version: version,
                    mailboxId: request.mailboxId
                },
                scopes: ['nullbox.alias.read-all'],
            }
        );
        return result.data.value;
    }
    const update = async (version: string, id: string, mailboxId: string, body: UpdateRequest, options?: any) => {
        await useApi(
            '/:version/mailboxes/:mailboxId/aliases/:id',
            {
                ...(options || {}),
                method: 'PATCH',
                body,
                params: {
                    version: version,
                    id: id,
                    mailboxId: mailboxId
                },
                scopes: ['nullbox.alias.update'],
            }
        );
    }

    return {
        create,
        get,
        getAll,
        update,
    }
}
