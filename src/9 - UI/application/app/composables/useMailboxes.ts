import type { CreateRequest } from "~/types/dto/mailboxes/create-request";
import type { CreateResponse } from "~/types/dto/mailboxes/create-response";
import type { GetAllResponse } from "~/types/dto/mailboxes/get-all-response";
import type { GetRequest } from "~/types/dto/mailboxes/get-request";
import type { GetResponse } from "~/types/dto/mailboxes/get-response";
import type { UpdateRequest } from "~/types/dto/mailboxes/update-request";

export function useMailboxes() {

    const create = async (version: string, body: CreateRequest, options?: any) => {
        const result = await useApi<CreateResponse>(
            '/:version/mailboxes',
            {
                ...(options || {}),
                method: 'POST',
                body,
                params: {
                    version: version
                },
                scopes: ['nullbox.mailbox.create'],
            }
        );
        return result.data.value;
    }
    const get = async (version: string, request: GetRequest, options?: any) => {
        const result = await useApi<GetResponse>(
            '/:version/mailboxes/:id',
            {
                ...(options || {}),
                method: 'GET',
                params: {
                    version: version,
                    id: request.id
                },
                scopes: ['nullbox.mailbox.read'],
            }
        );
        return result.data.value;
    }
    const getAll = async (version: string, options?: any) => {
        const result = await useApi<GetAllResponse>(
            '/:version/mailboxes',
            {
                ...(options || {}),
                method: 'GET',
                params: {
                    version: version
                },
                scopes: ['nullbox.mailbox.read-all'],
            }
        );
        return result.data.value;
    }
    const update = async (version: string, id: string, body: UpdateRequest, options?: any) => {
        await useApi(
            '/:version/mailboxes/:id',
            {
                ...(options || {}),
                method: 'PATCH',
                body,
                params: {
                    version: version,
                    id: id
                },
                scopes: ['nullbox.mailbox.update'],
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
