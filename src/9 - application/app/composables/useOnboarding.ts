import type { CreateUserProfileRequest } from "~/types/dto/onboarding/create-user-profile-request";

export function useOnboarding() {

    const createUserProfile = async (version: string, body: CreateUserProfileRequest, options?: any) => {
        await useApi(
            '/:version/on-board/user',
            {
                ...(options || {}),
                method: 'POST',
                body,
                params: {
                    version: version
                },
                scopes: ['nullbox.user.on-board'],
            }
        );
    }

    return {
        createUserProfile,
    }
}
